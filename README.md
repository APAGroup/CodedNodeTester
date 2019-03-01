# Środowisko tworzenia węzłów definiowalnych

Środowiskiem programowania węzłów jest Visual Studio (obecnie wersja 2017). Projekt testowy można uruchamiać w tym środowisku w celu testowania kodów. W  projekcie można produkować gotowe skompilowane biblioteki, które umożliwią szybsze uruchamianie się tych węzłów w aplikacji COR.

# Proces kompilacji

Solucja CodedNodeDesigner zawiera dwa projekty:

1. CodedNode.csproj - projekt ten zawiera kod tworzonego węzła definiowalnego. Programista w tym projekcie korzystając z przykładowego węzła ExampleCodedNode.cs - tworzy własną klasę “VisionDynamic”.
2. CodedNodeProj.csproj - projekt ten zawiera środowisko uruchomieniowe węzła definiowalnego w postaci aplikacji konsolowej, która dynamicznie tworzy klasę “VisionDynamic” zawartą w skompilowanej DLLce CodedNode.csproj, i pozwala na wywołanie na niej metod typu “Consume” w celu symulowania działania tego węzła.

 W aplikacji CFG możemy wykorzystać czysty kod węzła definiowalnego, lub - zastosować kod kompilowany dynamicznie w postaci CodedNode.dll (oczywiście można zmieniać jej nazwę).

Jeśli nasz kod będzie korzystał z bibliotek innych firm/organizacji (np. poprzez Nuget), należy pamiętać że jeśli te dllki są również używane w Nazce - muszą być one zgodne wersją. Pozostałe DLLki których nie ma w Nazce należy skopiować do katalogów Nazci, by zostały prawidłowo wpięte do węzła definiowalnego podczas jego uruchamiania. 