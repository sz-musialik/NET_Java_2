# Laboratorium 2 - Projekt aplikacji bazodanowej .NET
Aplikacja bazodanowa napisana w języku C# wykorzystująca wybrane API.

## API
Do projektu wybrano **James Webb Space Telescope API**, które umożliwia przegląd danych z teleskopu Jamesa Webb'a.

Klucz API został ukryty za pomocą pliku `.env`, który został wpisany do `.gitignore`.

## Baza danych
Baza danych została zaimplementowana przy pomocy **SQLite**.

Dane odebrane przy pomocy Endpointów API w formacie **JSON** zostały przyporządkowane odpowiednim polom tabel.

Nadpisana metoda `ToString()` w przystępny sposób wypisuje odczytane przez API dane.

## Opis działania
Program oferuje użytkownikowi odczyt danych udostępnianych przez API oraz ich zapis do lokalnej bazy danych i późniejszy odczyt.

Przy uruchomieniu programu wyświetlane jest Menu użytkownika umożliwiające:
- Wyświetlenie rekordów z danego programu badawczego,
- Wyświetlenie rekordów o zadanym typie pliku,
- Wyświetlenie rekordów zapisanych w lokalnej bazie danych.

Jeśli użytkownik chce odczytać dane, które istnieją już w bazie lokalnej są one wyświetlane, w innym przypadku wysyłane jest odpowiednie żądanie. W obecnej wersji program wyświetla 10 rekordów dla jednego żądania.

## Uruchomienie projektu
Aby uruchomić aplikację konsolową należy otworzyć projekt w środowisku Visual Studio oraz wybrać projekt `AplikacjaBazodanowa` jako projekt startowy i go uruchomić.
