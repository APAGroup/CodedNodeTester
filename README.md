# Środowisko tworzenia węzłów definiowalnych

### Wymagania

Testowanie węzłów w VS w projekcie testowym odbywa się w prosty sposób, czyli przez uruchamianie projektu (kompilacje) pod Debugerem. W oknie aplikacji testowej prezentowane są rezultaty testów tych węzłów. Kompilowanie do bibliotek odbywa się bardzo podobnie.

W eksploratorze rozwiązań, dla każdego ze swoich plików z kodem w katalogu Codes projektu, należy we właściwościach wybrać opcję:

Kopiuj do katalogu wyjściowego → Zawsze kopiuj

![eksplorator](https://i.ibb.co/Rzx9fjw/eksploratorrozwiazan.png)

### Proces kompilacji

Aby skompilować naszą bibliotekę “ExampleCodedNode.cs”, skompilowaną wcześniej aplikację CodedNodeTester uruchomić z linii poleceń z parametrami:

 `CodedNodeTester.exe -c ExampleCodeNode`

Spowoduje to uruchomienie aplikacja testowej w trybie kompilacji. W katalogu docelowym tej aplikacji (znajduje się w katalogu projektu:“CodedNodeTester\bin\Debug”) aplikacja utworzy dwa katalogi:

* Codes - tutaj kopiowane są pliki z kodami węzłów, można do niej dorzucać dodatkowe pliki, nie uwzględnione w projekcie,
* Compiled - tutaj wylądują skompilowane pliki bibliotek. Te gotowe biblioteki dostarczamy do repozytorium mediów w aplikacji COR.

Jeśli w parametrach wywołań pominiemy nazwę naszej biblioteki, to aplikacji skompiluje wszystkie pliki znajdujące się w katalogu Codes (opcja ułatwiająca życie przy przejściu na tę wersję).

Oczywiście kompilację można też przeprowadzić z poziomu środowiska VS. W tym celu wystarczy we właściwościach projektu, w sekcji Debug, wprowadzić nasze parametry wywołania aplikacji:

![ustawienia](https://i.ibb.co/Lpm6Jjr/ustawieniaprojektu.png)

Jeśli aplikacja testowe napotka błędy podczas kompilacji, stosownie wypisze je podczas tego procesu. Przy kompilacji wielu plików, wystąpienie błędów w jednym z nich **nie przerwie procesu kompilacji**.

W aplikacji CFG możemy wykorzystać czysty kod węzła definiowalnego, lub - zastosować kod kompilowany dynamicznie.
