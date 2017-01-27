Filip Nienartowicz
inf122531
Zaj�cia: wtorek 16:50

1. Temat: 1. Gra logiczna lub zr�czno�ciowa (backgammon)

2. Metoda realizacji projektu:
Przyj�ta realizacja projektu to protok� TCP ze sta�ym (wynosz�cym 15 znak�w) komunikatem.

Komunikaty sk�adaj� si� z:
- znaku C (client) lub S (server) - �r�d�o komunikatu
- znaku typu wiadomo�ci: I - informacyjne, G - gra, E - b��d
- 2 znak�w (kod komunikatu): liczba 00 - 99. W po��czeniu z powy�szym jednoznacznie identyfikuje komunikat.
- 11 znak�w dodatkowych (w zale�no�ci od komunikatu wype�nia si� je odpowiednimi informacjami)

np. CI10asdf - klient przesy�a wiadomo�� informacyjn� o kodzie 10 (Hello server). Podaje w niej r�wnie� sw�j nick (max 10 znak�w, mo�e sk�ada� si� z liczb, cyfr i '_') - logowanie.
SG4010060 - server przesy�a ruch przeciwnika. W tym wypadku przeciwnik przesun�� pionek z pola 10 na 6, ko�cowe 0 oznacza, �e nie jest to jeszcze koniec jego tury.

Pe�en spis komunikat�w jest dost�pny w pliku "Komuniakcja - komunikaty" za��czonym do repozytorium.

3. Opis implementacji:
1) Klient:
Klient zosta� zaimplementowany w j�zyku c w systemie Linux (Ubuntu). Do implementacji wykorzystano funkcj� select.

Pliki �r�d�owe: server.c (w folderze server)

2) Serwer:
Serwer zosta� zaimplementowany w j�zyku c# (.NET) z wykorzystaniem oprogramowania Visual Studio 2013

Pliki �r�d�owe: projekt znajduje sie w folderze client/Backgammon. W folderze Grafika znajduj� si� grafiki wykorzystywane w trakcie trwania programu.

4. Spos�b kompilacji, uruchomienia i obs�ugi program�w projektu:
1) KLient: gcc -Wall server.c -o server

2) Serwer:
Kompilacja przy u�yciu programu Visual Studio 2013.
Uruchomienie: client/Backgammon/Backgammon/bin/Release/Backgammon.exe

Obs�uga:
W pierwszym oknie nale�y poda� nick gracza oraz adres IP serwera
Aby rozpocz�� now� gr� nale�y klikn�� new game.
Przycisk roll dices powoduje wylosowanie ko�ci.
Pionki przesuwa si� za pomoc� myszy klikaj�c na wybrane pole z pionkami w kolorze gracza, a nast�pnie na inne pole, gdzie ma zosta� przeniesiony pionek (po wybraniu pola startowego na planszy wy�wietlane s� podpowiedzi, gdzie mo�na przesun�� pionek)
Po zako�czonej grze gracz ma mo�liwo�� rozegrania kolejnej gry z tym samym przeciwnikiem - obie strony musz� si� zgodzi�. W tym celu nale�y klikn�� przycisk zgody w oknie nowej gry.
Aby zako�czy� prac� z programem nale�y klikn�� przycisk Exit.