Filip Nienartowicz
inf122531
Zajêcia: wtorek 16:50

1. Temat: 1. Gra logiczna lub zrêcznoœciowa (backgammon)

2. Metoda realizacji projektu:
Przyjêta realizacja projektu to protokó³ TCP ze sta³ym (wynosz¹cym 15 znaków) komunikatem.

Komunikaty sk³adaj¹ siê z:
- znaku C (client) lub S (server) - Ÿród³o komunikatu
- znaku typu wiadomoœci: I - informacyjne, G - gra, E - b³¹d
- 2 znaków (kod komunikatu): liczba 00 - 99. W po³¹czeniu z powy¿szym jednoznacznie identyfikuje komunikat.
- 11 znaków dodatkowych (w zale¿noœci od komunikatu wype³nia siê je odpowiednimi informacjami)

np. CI10asdf - klient przesy³a wiadomoœæ informacyjn¹ o kodzie 10 (Hello server). Podaje w niej równie¿ swój nick (max 10 znaków, mo¿e sk³adaæ siê z liczb, cyfr i '_') - logowanie.
SG4010060 - server przesy³a ruch przeciwnika. W tym wypadku przeciwnik przesun¹³ pionek z pola 10 na 6, koñcowe 0 oznacza, ¿e nie jest to jeszcze koniec jego tury.

Pe³en spis komunikatów jest dostêpny w pliku "Komuniakcja - komunikaty" za³¹czonym do repozytorium.

3. Opis implementacji:
1) Klient:
Klient zosta³ zaimplementowany w jêzyku c w systemie Linux (Ubuntu). Do implementacji wykorzystano funkcjê select.

Pliki Ÿród³owe: server.c (w folderze server)

2) Serwer:
Serwer zosta³ zaimplementowany w jêzyku c# (.NET) z wykorzystaniem oprogramowania Visual Studio 2013

Pliki Ÿród³owe: projekt znajduje sie w folderze client/Backgammon. W folderze Grafika znajduj¹ siê grafiki wykorzystywane w trakcie trwania programu.

4. Sposób kompilacji, uruchomienia i obs³ugi programów projektu:
1) KLient: gcc -Wall server.c -o server

2) Serwer:
Kompilacja przy u¿yciu programu Visual Studio 2013.
Uruchomienie: client/Backgammon/Backgammon/bin/Release/Backgammon.exe

Obs³uga:
W pierwszym oknie nale¿y podaæ nick gracza oraz adres IP serwera
Aby rozpocz¹æ now¹ grê nale¿y klikn¹æ new game.
Przycisk roll dices powoduje wylosowanie koœci.
Pionki przesuwa siê za pomoc¹ myszy klikaj¹c na wybrane pole z pionkami w kolorze gracza, a nastêpnie na inne pole, gdzie ma zostaæ przeniesiony pionek (po wybraniu pola startowego na planszy wyœwietlane s¹ podpowiedzi, gdzie mo¿na przesun¹æ pionek)
Po zakoñczonej grze gracz ma mo¿liwoœæ rozegrania kolejnej gry z tym samym przeciwnikiem - obie strony musz¹ siê zgodziæ. W tym celu nale¿y klikn¹æ przycisk zgody w oknie nowej gry.
Aby zakoñczyæ pracê z programem nale¿y klikn¹æ przycisk Exit.