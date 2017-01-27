#include <netdb.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>
#include <arpa/inet.h>
#include <netinet/in.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <time.h>
#include <stdlib.h>
#include <ctype.h>

#define ERROR(e) { perror(e); exit(EXIT_FAILURE); }
#define SERVER_PORT 3000
#define QUEUE_SIZE 5

//Dlugosc wiadomosci, max liczba klientow, max czas od ostatniej wiadomosci (przekroczenie oznacza usuniecie klienta)
const int maxn = 15;
const int maxuser = 100; 
const double maxcommgap = 60 * 1000;

//Lista niepobranych przez klienta ruchow przeciwnika (wraz z obsluga)
struct SMoveList{
	char mess[15];
	struct SMoveList * next;
};

//Tworzy element listy ruchow przeciwnika
struct SMoveList* NewMoveList(char mess[15])
{
	struct SMoveList *list = (struct SMoveList*) malloc(sizeof( struct SMoveList));
	list->next = NULL;
	memcpy(list->mess, mess, 15);
	return list;
}
 
//Pobiera pierwszy ruch z listy, zwraca tresc i usuwa go
struct SMoveList* GetMoveListElement(struct SMoveList* list, char mess[15])
{
	if(list != NULL)
	{
		struct SMoveList* newlist = list;
		list = list->next;
		memcpy(mess, newlist->mess, 15);
		free(newlist);
		return list;
	}
	return NULL;
}

//Dodaje element na koniec listy
struct SMoveList* AddMoveToList(struct SMoveList* list, struct SMoveList* element)
{
	if(list != NULL)
	{
		struct SMoveList* pop, *act = list;
		while(act!=NULL)
		{
			pop = act;
			act = act->next;
		}
		pop->next = element;
		return list;
	}
	else
	{
		return element;
	}
}

//Tworzy element listy z wiadomosci i dodaje go na koniec listy
struct SMoveList* AddMoveToListMess(struct SMoveList* list, int color, char mess[15])
{
	struct SMoveList* element = NewMoveList(mess);
	return AddMoveToList(list, element);
}

//Czysci liste
void ClearMoveList(struct SMoveList* list)
{
	while(list != NULL)
	{
		struct SMoveList* cur = list;
		list = list->next;
		free(cur);
	}
}
//////////////////////////////////////////

//Struktura przechowujaca informacje o grze
struct SGame{
	//w tab 0 - white, 1 - red
	int players[2]; //przechowuje id klienta, 0 jesli klient opuscil juz gre
	int playerstatus[2]; //0 - gra, 1 - rezygnuje, 2 - zgadza sie na kolejna rozgrywke, 3 - gracz posiada nowa gre 
	int score[3]; //2 - czy ktos juz podbil (unikniecie kilkukrotnego wyslania komuniaktu end game, ze zmiana wyniku
	int move; //0 - white, 1 - red, 2 - game starts, 3 - end game
	int startdice[3]; //0 w startowej oznacza, ze przeciwnik jeszcze nie wylosowal, 3 pole to wynik poprzedniego remisu
	int gotstartdice[2]; //0 - nie losowal, 1 - losowal, 2 - byl remis
	int dice[2][2]; //0-6, 0 - jeszcze nie byly losowane
	struct SMoveList *unreadmoves[2];
};

//Tworzy nowa gre
struct SGame* NewGame(int white, int red, int scwhite, int scred)
{
	struct SGame *game = (struct SGame*) malloc(sizeof(struct SGame));
	game->players[0] = white;
	game->players[1] = red;
	game->playerstatus[0] = 0;
	game->playerstatus[1] = 0;
	game->score[0] = scwhite;
	game->score[1] = scred;
	game->score[2] = 0;
	game->move = 2;
	game->startdice[0] = 0;
	game->startdice[1] = 0;
	game->startdice[2] = 0;
	game->gotstartdice[0] = 0;
	game->gotstartdice[1] = 0;
	
	game->dice[0][0] = 0;
	game->dice[0][1] = 0;
	game->dice[1][0] = 0;
	game->dice[1][1] = 0;
	
	game->unreadmoves[0] = NULL;
	game->unreadmoves[1] = NULL;
	return game;
}

/*
Usuwa gracza (color) z gry, jesli obaj gracze opuscili gre, to usuwa takze gre
color - 0 white, 1 - red
zwraca - 0, gdy powiodlo sie usuniecie gry, 1 - drugi gracz jeszcze nie skonczyl gry, 2 - color != {0,1}, 3 - game pusty
*/
int DeleteGame(struct SGame *game, int color)
{
	if(game!=NULL)
	{
		if(color == 0 || color == 1)
		{
			game->playerstatus[color] = 1;
			if(game->playerstatus[1 - color] == 1)
			{
				ClearMoveList(game->unreadmoves[0]);
				ClearMoveList(game->unreadmoves[1]);
				printf("Zwalniam gre\n");
				free(game);
				return 0;
			}
			return 1;
		}
		return 2;
	}
	return 3;
}

/*
Zwraca kolor gracza w grze
client - id klienta
2 - klient nie nalezy do gry, 0 - white, 1 - red, 
client to fd klienta
*/
int GetGameColor(struct SGame *game, int client)
{
	if(game->players[0] == client)
	{
		return 0;
	}
	if(game->players[1] == client)
	{
		return 1;
	}
	return 2;
}

//Zmiana tury - wlacznie z pierwsza
void NewMove(struct SGame *game)
{
	if(game->move != 3)
	{
		if(game->move == 2)
		{
			if(game->startdice[0] > 0 && game->startdice[1] > 0)
			{
				if(game->startdice[0] > game->startdice[1])
				{
					game->move = 0;
					game->dice[game->move][0] = 0;
					game->dice[game->move][1] = 0;
				}
				if(game->startdice[0] < game->startdice[1])
				{
					game->move = 1;
					game->dice[game->move][0] = 0;
					game->dice[game->move][1] = 0;
				}
			}
		}
		else
		{
			game->move = 1 - game->move;
			game->dice[game->move][0] = 0;
			game->dice[game->move][1] = 0;
		}
	}
}

//Rzut koscia
int RollDice()
{
	return rand()%6+1;
}

//Podaje kosci startowe graczy
void GetStartDice(struct SGame *game, int color, int dice[2])
{
	dice[0] = 0;
	dice[1] = 0;
	
	if(color == 0 || color == 1)
	{
		switch(game->gotstartdice[color])
		{
			//nie wylosowal
			case 0: 
			{
				game->startdice[color] = RollDice();
				game->gotstartdice[color] = 1;
				
				dice[0] = game->startdice[color];
				dice[1] = game->startdice[1 - color];
				
				if(dice[0] == dice[1])
				{
					game->gotstartdice[color] = 0;
					game->gotstartdice[1 - color] = 2;
					game->startdice[2] = dice[0];
					game->startdice[0] = 0;
					game->startdice[1] = 0;
				}
				else
				{
					NewMove(game);
				}
			}break;
			
			//wylosowal
			case 1: 
			{
				dice[0] = game->startdice[color];
				dice[1] = game->startdice[1 - color];
			}break;
			
			//byl remis
			case 2: 
			{
				game->gotstartdice[color] = 0;
				
				dice[0] = game->startdice[2];
				dice[1] = game->startdice[2];
				game->startdice[2] = 0;
			}break;
			
			default:;break;
		}
	}
}

//Podaje kosci gracza (jak niewylosowane, to losuje)
void GetDices(struct SGame *game, int color, int dice[2])
{
	dice[0] = 0;
	dice[1] = 0;
	if(game->dice[color][0] == 0)
	{
		game->dice[color][0] = RollDice();
		game->dice[color][1] = RollDice();
	}
	dice[0] = game->dice[color][0];
	dice[1] = game->dice[color][1];
}

/*
Podaje kosci przeciwnika
color - kolor gracza, a nie przeciwnika!
*/
void GetOpponentdices(struct SGame *game, int color, int dice[2])
{
	dice[0] = game->dice[1 - color][0];
	dice[1] = game->dice[1 - color][1];
}
/////////////////////////////

//Struktura przechowujaca dane klienta
struct SClient {
	char ans[15]; //odpowiedz serwera
	clock_t lastmessage;
	int loggedin; //0 - niezalogowany, 1 - zalogowany, 2 - do usuniecia
	char nick[10]; //nick gracza
	int opponent; //0 - no, 1 - looking for, id - id of opponent
	struct SGame * game;
}client[100];

//Klient, ktory nie ma przeciwnika
int waitingforopponet = 0;

//Zwraca id przeciwnika, 0 - jesli nie ma dostepnych przeciwnikow i dodaje gracza do listy
int GiveOpponent(int i)
{
	if(client[i].opponent > 1)
	{
		return client[i].opponent;
	}
	
	if(client[i].opponent == 0)
	{
		if(waitingforopponet > 0)
		{
			int id = waitingforopponet;
			waitingforopponet = 0;
			
			client[i].opponent = id;
			client[id].opponent = i;
			return id;
		}
		else
		{
			waitingforopponet = i;
			client[i].opponent = 1;
			return 0;
		}
	}
	return 0;
}
////////////////////////////////

//Przygotowuje pusta tablice klientow do pracy serwera
void ClientListIniciation()
{
	int i, k;
	for(i = 0; i<100; i++)
	{
		client[i].loggedin = 0;
		for(k = 0; k < 15; k++)
			client[i].ans[k] = 0;
		for(k = 0; k < 10; k++)
			client[i].nick[k] = 0;
		client[i].opponent = 0;
		client[i].game = NULL;
	}
}

//Przygotowuje odpowiedz serwera dla klienta i
void PrepareServerMessage(int i, char* mess, int size)
{
	//czyszczenie poprzedniej wiadomosci
	char a[15] = {0};
	memcpy(client[i].ans, a, 15);
	memcpy(client[i].ans, mess, size);
}

//Usuwa gracza z gry oraz czysci pole opponent
void LeaveGame(struct SGame * game, int i)
{
	printf("Klient %d opuszcza gre\n", i);
	int color = GetGameColor(game, i);
	DeleteGame(game, color);
	client[i].opponent = 0;
	client[i].game = NULL;
}

//Czysci informacje o kliencie
void RemoveClient(int i)
{
	printf("Usuwam informacje o klienie %d\n", i);
	
	char mess[1] = {0};
	PrepareServerMessage(i, mess, sizeof(mess));
	client[i].lastmessage = 0;
	client[i].loggedin = 0;
	char a[10] = {0};
	memcpy(client[i].nick, a, 10);
	if(client[i].opponent!=0)
	{
		if(waitingforopponet == i)
		{
			waitingforopponet = 0;
		}
		else
		{
			if(client[i].game!=NULL)
			{
				printf("e\n");
				LeaveGame(client[i].game, i);
			}
		}
		client[client[i].opponent].opponent = 0;
		client[i].opponent = 0;
	}
}

const char *types[] = {"CI", "CG", "CE"};

/*
Interpretuje otrzymana wiadomosc, odpowiednio na nia reaguje i przygotowuje wiadomosc zwrotna
Praktycznie cala logika przesylu komunikatow jest tutaj
zwraca: 0 - jest ok, 1 - blad (wiadomosc typu error)
*/
int UnpackMessage(int i, char* mess)
{
	//Aktualizuje czas ostatniej wiadomosci otrzymanej od klienta
	client[i].lastmessage = clock();
	
	/*typ wiadomosci. Wyrozniamy: CI - informacyjne, CG - w czasie gry, CE - bledy 
	w polaczeniu z numerem wiadomosci stanowia identyfikator komunikatu nadanego przez klienta
	Postaram sie przy kazdym identyfikatorze wiadomosci podac pelna nazwe, w celu prostszego odbioru kodu.
	Pelen spis identyfikatorow umieszczony jest w pliku Komunikacja-komunikaty umieszczonym w repozytorium*/
	
	char messtype[3]= {0};
	memcpy(messtype, mess, 2);
	
	//numer wiadomosci (zakres 00 - 99) w kazdej kategorii (istnieje mozliwosc rozbudowy systemu o kolejne)
	int code = (mess[2] - '0')*10 + mess[3] - '0';
	
	//Informacyjne CI
	if(strcmp(types[0], messtype) == 0)
	{
		//printf("Rodzaj wiadomosci: INFORMACYJNA\n");

		switch(code)
		{
			//CI10_nick - hello server
			case 10: 
			{
				int pop = 1;
				int k;
				
				if(!isalnum(mess[4]))
				{
					pop = 0;
				}
				for(k = 5; k <14; k++)
				{
					if(!isalnum(mess[k]) && mess[k] != 0 && mess[k]!= '_')
					{
						pop = 0;
					}
				}
				
				if(pop)
				{
					memcpy(client[i].nick, mess + 4, 10);
					client[i].loggedin = 1;
				
					printf("Klient %d otrzymal nick: %s\n", i, client[i].nick);
					//SI10 - Hello client
					PrepareServerMessage(i, "SI10", 4);
				}
				else
				{
					printf("Klient %d przeslal niepoprawny nick\n", i);
					//SI11 - Wrong nick
					PrepareServerMessage(i, "SI11", 4);
				}
				return 0;
			}break;
			
			//CI20 - logout
			case 20: 
			{
				printf("Klient %d konczy komunikacje\n", i);
			
				client[i].loggedin = 2;
				
				//SI20 - logout accept
				PrepareServerMessage(i, "SI20", 4);
				return 0;
			}
			
			//CI30 - still here (wysylana przez klienta, zeby podtrzymac komunikacje i nie zostac usunietym)
			case 30:
			{
				//printf("Klient %d still here\n", i);
				
				//SI30 - Accept still here
				PrepareServerMessage(i, "SI30", 4);
				return 0;
			}break;
			
			//CI40 - give opponent
			case 40:
			{
				if(client[i].loggedin == 1)
				{
					int opponent = GiveOpponent(i);
					printf("Klient %d przeciwnik %d\n", i, opponent);
					
					if(opponent < 2)
					{
						//SI41 - no opponent available
						PrepareServerMessage(i, "SI41", 4);
						return 0;
					}
					else
					{
						//nowa gra
						if(client[i].game == NULL)
						{
							printf("Klient %d tworzy nowa gre\n", i);
							struct SGame* game = NewGame(i, opponent, 0, 0);
							client[i].game = game;
							client[opponent].game = game;
						}
						//SI40_nick_przeciwnika - opponent
						char sermess[15] = {0}, messnum[4] = "SI40";
						memcpy(sermess, messnum, 4);
						memcpy(sermess+4, client[opponent].nick, 10);
						PrepareServerMessage(i, sermess, sizeof(sermess));
						return 0;
					}
				}
				printf("Klient %d klient nie jest zalogowany\n", i);
				//SE20 - client not logged in
				PrepareServerMessage(i, "SE20", 4);
				return 1;
			}break;
			
			//Nie rozpoznano wiadomosci
			default: 
			{
				printf("Klient %d przeslal nieznana wiadomosc!!!\n", i);
				//SE10 - unknown message
				PrepareServerMessage(i, "SE10", 4);
			}break;
		}
		return 1;
	}
	
	//Gra CG
	if(strcmp(types[1], messtype) == 0)
	{
		//printf("Rodzaj wiadomosci: GRA\n");
		
		//Czy klient zalogowany
		if(client[i].loggedin == 1)
		{
			//Czy klient posiada gre
			if(client[i].game != NULL)
			{
				//gra i kolor klienta
				struct SGame * game = client[i].game;
				int color = GetGameColor(game, i);
				
				//Przeciwnik opuscil gre
				if(game->playerstatus[1 - color] == 1)
				{
					printf("Klient %d Przeciwnik opuscil gre\n",i);
					LeaveGame(game, i);
					//SE60 - opponent surrender
					PrepareServerMessage(i, "SE60", 4);
					return 1;
				}
				
				switch(code)
				{
					//CG10 - give new game
					case 10: 
					{
						printf("Klient %d new game\n", i);
						
						//SG10 - new game (kolor graca, wynik gracza i przeciwnika (wynik z zakresu 0-99)
						char sermess[15] = {0}, messnum[4] = "SG10";
						memcpy(sermess, messnum, 4);
						
						sermess[4] = '0' + color;
						sermess[5] = '0' + game->score[color]/10;
						sermess[6] = '0' + game->score[color]%10;
						sermess[7] = '0' + game->score[1 - color]/10;
						sermess[8] = '0' + game->score[1 - color]%10;
						PrepareServerMessage(i, sermess, sizeof(sermess));
						return 0;
					}break;
					
					//CG11 - give start dices
					case 11: 
					{
						int dices[2];
						GetStartDice(game, color, dices);
						
						printf("Klient %d kosci startowe %d %d\n", i, dices[0], dices[1]);
						
						//SG11 - start dices (kosc gracza, kosc przeciwnika - 0 jeśli przeciwnik nie rzucil kosci
						char sermess[15] = {0}, messnum[4] = "SG11";
						memcpy(sermess, messnum, 4);
						
						sermess[4] = '0' + dices[0];
						sermess[5] = '0' + dices[1];
						
						PrepareServerMessage(i, sermess, sizeof(sermess));
						return 0;
					}break;
					
					//CG20 - give my dices
					case 20: 
					{
						if(game->move < 2)
						{
							if(game->move == color)
							{
								int dices[2];
								GetDices(game, color, dices);
								
								printf("Klient %d kosci %d %d\n", i, dices[0], dices[1]);
								
								//SG20 - start dices
								char sermess[15] = {0}, messnum[4] = "SG20";
								memcpy(sermess, messnum, 4);
								
								sermess[4] = '0' + dices[0];
								sermess[5] = '0' + dices[1];
								
								PrepareServerMessage(i, sermess, sizeof(sermess));
								return 0;
							}
							else
							{
								//SE40 - not your turn
								PrepareServerMessage(i, "SE40", 4);
								return 1;
							}
						}
						else
						{
							if(game->move == 2)
							{
								//SE50 - game not started
								PrepareServerMessage(i, "SE50", 4);
								return 1;
							}
							else
							{
								//SE53 - game ended
								PrepareServerMessage(i, "SE53", 4);
								return 1;
							}
						}
					}break;
					
					//CG21 - give opponent dices
					case 21: 
					{
						if(game->move != 2)
						{
							int dices[2];
							GetOpponentdices(game, color, dices);
							
							printf("Klient %d kosci przeciwnika %d %d\n", i, dices[0], dices[1]);
							
							//SG21 - opponent dices
							char sermess[15] = {0}, messnum[4] = "SG21";
							memcpy(sermess, messnum, 4);
							
							sermess[4] = '0' + dices[0];
							sermess[5] = '0' + dices[1];
							
							PrepareServerMessage(i, sermess, sizeof(sermess));
							return 0;
						}
						else
						{
							if(game->move == 2)
							{
								//SE50 - game not started
								PrepareServerMessage(i, "SE50", 4);
								return 1;
							}
						}
					}break;
					
					//CG30 - Move(z,do, koniec tury) - z i do to pola planszy, 2 znaki na kazdy, koniec tury: 0 - nie, 1 - tak
					case 30: 
					{
						if(game->move < 2)
						{
							if(game->move == color)
							{
								if(game->dice[color][0] != 0)
								{
									if(game->unreadmoves[color] == NULL)
									{
										printf("Klient %d ruch\n", i);
										
										//SG40 - Opponent move(z, do, koniec) - kopiuje wiadomosc do listy nieodczytanych przez przeciwnika
										char sermess[15] = {0}, messnum[4] = "SG40";
										memcpy(sermess, mess, 15);
										memcpy(sermess, messnum, 4);
										game->unreadmoves[1 - color] = AddMoveToListMess(game->unreadmoves[1 - color], color, sermess);
										
										//Koniec tury
										if(mess[8] == '1')
										{
											printf("Klient %d koniec tury\n", i);
											NewMove(game);
										}
										//SG30 - Move accept
										PrepareServerMessage(i, "SG30", 4);
										return 0;
									}
									else
									{
										//SE52 Client’s unread moves not empty
										PrepareServerMessage(i, "SE52", 4);
										return 1;
									}
								}
								else
								{
									//SE51 client hasn't rool dices
									PrepareServerMessage(i, "SE51", 4);
									return 1;
								}
							}
							else
							{
								//SE40 - not client turn
								PrepareServerMessage(i, "SE40", 4);
								return 1;
							}
						}
						else
						{
							if(game->move == 2)
							{
								//SE50 - game not started
								PrepareServerMessage(i, "SE50", 4);
								return 1;
							}
							else
							{
								//SE53 - game ended
								PrepareServerMessage(i, "SE53", 4);
								return 1;
							}
						}
					}break;
					
					//CG40 - give opponent move
					case 40:
					{
						if(game->move != 2)
						{
							if(game->unreadmoves[color] == NULL)
							{
								if(game->move == color)
								{
									//SG42 - no opponent move - do your turn
									PrepareServerMessage(i, "SG42", 4);
									return 0;
								}
								else
								{
									//SG41 - Opponent move not ready
									PrepareServerMessage(i, "SG41", 4);
									return 0;
								}
							}
							else
							{
								printf("Klient %d odczyt ruchu przeciwnika\n", i);
								char sermess[15] = {0};
								game->unreadmoves[color] = GetMoveListElement(game->unreadmoves[color], sermess);
								PrepareServerMessage(i, sermess, sizeof(sermess));
								return 0;
							}
						}
						else
						{
							//SE50 - game not started
							PrepareServerMessage(i, "SE50", 4);
							return 1;
						}
					}break;
					
					//CG50_zwyciezca - End Game
					case 50: 
					{
						printf("Klient %d zakonczyl gre\n", i);
						if(game->score[2] == 0)
						{
							if(mess[4] - '0' == 0 || mess[4] - '0' == 1)
							{
								game->score[mess[4]-'0'] += 1;
								game->score[2] = 1;
							}
							game->move = 3;
						}
						//SG50 - end game accept
						PrepareServerMessage(i, "SG50", 4);
						return 0;
					}break;
					
					//CG60_decyzja - Next Game 0 - rezygnacja, 1 - gra dalej
					case 60: 
					{
						if(game->move == 3)
						{
							if(mess[4] == '0')
							{
								LeaveGame(game, i);
							}
							else
							{
								printf("Klient %d chce grac dalej\n", i);
								game->playerstatus[color] = 2;
							}
							//SG60 - next game ack
							PrepareServerMessage(i, "SG60", 4);
							return 0;
						}
						else
						{
							//SE54 - Game hasn’t end yet
							PrepareServerMessage(i, "SE54", 4);
							return 1;
						}
					}break;
					
					//CG61 - give opponent decision
					case 61: 
					{
						if(game->move == 3)
						{
							if(game->playerstatus[1 - color] >= 2)
							{
								printf("Klient %d przeciwnik gra dalej\n", i);
								
								if(game->playerstatus[color] == 2)
								{
									if(game->playerstatus[1 - color] == 2)
									{
										struct SGame * newgame = NewGame(game->players[0], game->players[1], game->score[0],  game->score[1]);
										LeaveGame(game, i);
										printf("Klient %d tworzy nowa gre\n", i);
										client[i].game = newgame;
										game->playerstatus[color] = 3;
									}
									else
									{
										struct SGame * newgame = client[client[i].opponent].game;
										LeaveGame(game, i);
										printf("Klient %d dolacza do nowej gry\n", i);
										client[i].game = newgame;
									}
								}
								//SG61_decyzja - opponent decision
								PrepareServerMessage(i, "SG611", 5);
								return 0;
							}
							printf("Klient %d opponent jeszcze nie zdecydowal\n", i);
							//SG62 - opponent made no decision
							PrepareServerMessage(i, "SG62", 4);
							return 0;
						}
						else
						{
							//SE54 - Game hasn’t end yet
							PrepareServerMessage(i, "SE54", 4);
							return 1;
						}
					}break;
					
					//CG60 - surrender
					case 70: 
					{
						printf("Klient %d rezygnuje z gry\n", i);
						LeaveGame(game, i);
						//SG70 - surrender ack
						PrepareServerMessage(i, "SG70", 4);
						return 0;
					}break;
					
					//CG70 - not ready
					case 80: 
					{
						printf("Klient %d nie jest gotowy\n", i);
						//SG80 - not ready ack
						PrepareServerMessage(i, "SG80", 4);
						return 0;
					}break;
					
					//Nie rozpoznano wiadomosci
					default: 
					{
						printf("Klient %d przeslal nieznana wiadomosc!!!\n", i);
						//SE10 - unknown message
						PrepareServerMessage(i, "SE10", 4);
					}break;
				}
			}
			else
			{
				printf("Klient %d klient nie posiada gry\n", i);
				//SE30 - Client doesn’t have a game
				PrepareServerMessage(i, "SE30", 4);
			}
		}
		else
		{
			printf("Klient %d klient nie jest zalogowany\n", i);
			//SE20 - client not logged in
			PrepareServerMessage(i, "SE20", 4);
		}
		return 1;
	}
	
	//Bledy CE
	if(strcmp(types[2], messtype) == 0)
	{
		//printf("Rodzaj wiadomosci: ERROR\n");
		
		switch(code)
		{
			//CE10 - unknown message
			case 10: 
			{
				printf("Klient %d niezrozumial wiadomosci\n", i);
				//SE11 - unknown message ack
				PrepareServerMessage(i, "SE11", 4);
				return 0;
			}break;
			
			//Nie rozpoznano wiadomosci
			default: 
			{
				printf("Klient %d przeslal nieznana wiadomosc!!!\n", i);
				//SE10 - unknown message
				PrepareServerMessage(i, "SE10", 4);
				return 0;
			}break;
		}
		return 0;
	}
	printf("Klient %d przeslal nieznana wiadomosc!!!\n", i);
	//SE10 - unknown message
	PrepareServerMessage(i, "SE10", 4);
	return 1;
}

////////////////////////////
/*Najnizza warstwa komunikacji 
wysylanie i odbior wiadomosci, przyjmowanie nowych klientow
usuwanie klientow, ktorzy sie rozlaczyli
*/

//Wysyla komunikat do klienta
void write_loop(int fd)
{
    int i = 0;
    while(i < maxn)
    {
        i += write(fd, client[fd].ans + i, maxn-i);
    }
}

/*
Odbiera komunikat od klienta
zwraca 0 - gdy udalo sie odebrac wiadomosc, 1 - gdy klient zerwal polaczenie
*/
int read_loop(int fd, char * buffer)
{
    int i = 0;
int error = 0;
    while(i < maxn)
    {
		i += read(fd, buffer+i, maxn-i);
		if(i <= 0)
		{
			error++;
			if(error >= 10 || i == -1)
			{
				printf("Klient zerwal polaczenie\n");
				return 1;
			}
		}
    }
	return 0;
}

int main(int argc, char** argv)
{
	ClientListIniciation();
	srand(time(NULL));
	
	int i;
	
	socklen_t slt;
	int sfd, cfd, fdmax, fda, rc, on = 1;
	struct sockaddr_in saddr, caddr;
	static struct timeval timeout;

	memset(&saddr, 0, sizeof(saddr));
	saddr.sin_family = AF_INET;
	saddr.sin_addr.s_addr = INADDR_ANY;
	saddr.sin_port = htons(SERVER_PORT);

	if ((sfd = socket(AF_INET, SOCK_STREAM, 0)) < 0)
	ERROR("socket()")
	if (setsockopt(sfd, SOL_SOCKET, SO_REUSEADDR, (char*)&on, sizeof(on)) < 0)
	ERROR("setsockopt()")
	if (bind(sfd, (struct sockaddr*)&saddr, sizeof(saddr)) < 0)
	ERROR("bind()")
	if (listen(sfd, QUEUE_SIZE) < 0)
	ERROR("listen()")

	/*zbiory deskryptorow:
	-mask	-	wszystkie deskryptory
	-rmask i wmask	-	zbiory przekazywane do connect
	-allwmask	-	wszystkie, do ktorych serwer powinien teraz pisac
	-allrmask	-	wszystkie, od ktorych powinnismy dostac nowa informacje
	*/
	fd_set mask, rmask, wmask, allrmask, allwmask;
	FD_ZERO(&mask);
	FD_ZERO(&allrmask);
	FD_ZERO(&rmask);
	FD_ZERO(&allwmask);
	FD_ZERO(&wmask);
	fdmax = sfd;

	FD_SET(sfd, &allrmask);
	FD_SET(sfd, &mask);
	while(1)
	{
		rmask = allrmask;
		wmask = allwmask;

		timeout.tv_sec = 1 * 60;
		timeout.tv_usec = 0;
		if ((rc = select(fdmax+1, &rmask, &wmask, (fd_set*)0, &timeout)) < 0)
		  ERROR("select()")
		
		if (rc == 0)
		{
			  printf("time out\n");
			  continue;
		}

		fda = rc;
		
		//Nowy klient
		if (FD_ISSET(sfd, &rmask))
		{
			  fda -= 1;
			  slt = sizeof(caddr);
			  if ((cfd = accept(sfd, (struct sockaddr*)&caddr, &slt)) < 0)
				ERROR("accept()")
			  printf("new connection: %s\n",
					 inet_ntoa((struct in_addr)caddr.sin_addr));
			  client[cfd].lastmessage = clock();
			  FD_SET(cfd, &allrmask);
			  FD_SET(cfd, &mask);
			  if (cfd > fdmax) fdmax = cfd;
		}

		for (i = sfd+1; i <= fdmax && fda > 0; i++)
		{
			//Czytanie
			if (FD_ISSET(i, &rmask))
			{
				fda -= 1;
				char mess[maxn];
				if(!read_loop(i,mess))
				{
					printf("%d new read: %s\n", i, mess);
					UnpackMessage(i, mess);
					
					FD_CLR(i, &allrmask);
					FD_SET(i, &allwmask);
				}
				else
				{
					FD_CLR(i, &allrmask);
					client[i].loggedin = 2;
				}                  
			}

			//Pisanie
			if (FD_ISSET(i, &wmask))
			{
				fda -= 1;
				write_loop(i);
				printf("%d new write: %s\n", i, client[i].ans);
				
				FD_CLR(i, &allwmask);
				FD_SET(i, &allrmask);
			}
		}
		//Usuwanie klientow
		for(i = 0; i <= fdmax; i++)
		{
			clock_t time = clock();
			if(client[i].lastmessage != 0)
			{
				if(client[i].loggedin == 2 || (double)difftime(time, client[i].lastmessage) >= maxcommgap)
				{
					RemoveClient(i);
					close(i);
					FD_CLR(i, &mask);
					FD_CLR(i, &allrmask);
					FD_CLR(i, &allwmask);
					if (i == fdmax)
					{
						while(fdmax > sfd && !FD_ISSET(fdmax, &mask))
							fdmax -= 1;
					}
				}
			}
		}
		printf("end\n");
	}
	close(sfd);
	return EXIT_SUCCESS;
}
