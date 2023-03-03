# Lista uwag i propozycje rozwiązania: 

Cześć kolego ;) 
przejżałem twój kod i jak prosiłeś oto moje uwagi: 

1. Przy uruchomieniu programu rzucany jest wyjątek, z powodu błędnie podanej nazwy pliku do odczytu w metodzie Main
dataa.csv zamiast data.csv

2. W przygotowanych danych występują puste linie które powodują rzucanie wyjątkiem 
	propozycje rozwiązania: 
		- ominać puste linie z pliku z danymi raczej w momencie czytania pliku w petli while dodac warunek jeśli linia jest pusta to continue
		- aternatywne rozwiązanie przeze mnie zastosowane:
			wykorzystać inny mechanizm pobierania danych w pliku csv, np. CsvHelper ma wbudowane narzedzia do odczytu takich plików i może pominac takie puste linie 
		
3. w 366 linijce przygotowanych danych brakuje średnika (;) co powoduje zbyt małą ilość elementów w tablcy powstałej z metody split 
	propozycje rozwiązania: 
		- zabazpieczyć przed takimi sytuacjami, w przypadku braku danych ustawić np. null (przy pobieraniu danej wartości można weryfikowac czy tablica zawierająca dane values ma odpowiednią długość, lub takie niekompletne dane pomijac)
		- aternatywne rozwiązanie przeze mnie zastosowane:
			przy wykorzystaniu CsvHelper możemy okreslić co robić w przypadku błędnych danych lub braujących pól, co zostało zrobione i dodawane do osobnej listy invalidRecords, które na końcu metody moga być drukowane z jakimś komunikatem ostrzegającym o błednych danych 
		
4. pętla for wykracza poza granice pliku (plik csv jest liczony od 1 nie od 0) 
	propozycja rozwiązania:
		for (int i = 0; i < importedLines.Count; i++) // operator < zamiast <=

5. 16 linijka kodu niepotrzebny defaultowy obiekt w liście obiektów
	propozycja rozwiązania:
		ImportedObjects = new List<ImportedObject>();

===============
te 5 poprawek pozwala na wykonanie programu i wydrukowanie efektu na konsoli 
niestety z optymalizacyjnego punktu widzenia kod warto poprawić pod innymi względami niż poprawność oczekiwanego działnia 
===============

UWAGI Ogólne: 
6. Typy danych w klasie ImportedObject
w większości to string 
IsNullable powinno być typem bool? ->  1 lub nullable (bo takie dane znalazłem również w pliku) to true, jesli coś innego to false 

7. ImportedObjectBaseClass - posiada jedynie dwa pola Name i Type, a sam typ nie jest szczególnie używany jako jakieś rozróżnienie 
dodatkowo w klasie ImportedObject niepotrzebnie został zdefiniowany jedeno z właściwości 

ja zaproponowałem następujące rozdzielenie
wszystkie własciwości zostały z ImportedObject zostały przeniesione do ImportedObjectBaseClass oprócz NumberOfChildren - jako że te dane w rzeczywistości istnieją w importowanym pliku 
oczywiście definicje pól zastapione właściwościami, nalezy omijać publicznych pól 
następnie ImportedObject dziedzicząc po ImportedObjectBaseClass niepotrzebuje definiowania włąsciowści dziedziczonych
dlatego zdefiniowano tylko  NumberOfChildren (typ zmieniono na int, on zlicza wartości całkowite, double był niepotrzebny)
oraz dodano specjalne konstruktory jedne zwykły defaultowy 
oraz drugi który przyjmuje jako parametr obiekt ImportedObjectBaseClass oraz int jako wartość do inicjalizacji NumberOfChildren
dzięki temu możemy w takim konstruktorze przekazac od razu dane z ImportedObjectBaseClass do odpowiednich własciwości tworzonej klasy 

8. wszytskie klasy powinny znajdowac się w osobnych plikach dla większej przejżystosci jakie jak ImportedObject i ImportedObjectBaseClass

9. public void ImportAndPrintData(string fileToImport, bool printData = true)
niewykorzystywany argument printData, do usunięcia, nawet jeśli byłby uzywany to lepiej obie te funkcjonalności umieścić w osobnych metodach ImportData i PrintData
aby rozdzielić obie te funkcjonalności zgodnie z zasadami SOLID, odnosnie pojedyńczej odpowiedzialności


10. poprawić optymalizacje chyba najtrudniejsze z zadań: 
	sa wykonywane wielokrotnie pętle for i foreach do pewnego rodzaju przekształacania/poprawiania danych 
	
	zamiast interfejsu IEnumerable<ImportedObject> uzyo Listy List<ImportedObject> _importedObjects (prywatne pole z nazwą zaczynającą się od "_" zgodnie z pewnymi konwencjami, rzecz dyskusyjna)
	pozwoli to na ominięcie wielokrotnego inicjalizowannia takiej kolekcji w momencie jej wywołania 
	
	logika zliczania NumberOfChildren (pętle foreach w for n*n iteracji) 
		została zoptymalizowana dzięki jednej lini 
		_importedObjects = importedObjectsBase.Select(i => new ImportedObject(i, importedObjectsBase.Count(c => c.ParentType == i.Type && c.ParentName == i.Name))).ToList();
		już wewnątrz metody ImportData 
	
11. czyszczenie danych zostało zautomatyzowane już w momencie pobierania danych z pliku 
		wywołanie metod na wilu elementach Trim().Replace(" ", "").Replace(Environment.NewLine, "")
		zostało wyseparowane do metody rozszerzającej 
		 public static string FullTrim(this string s)
        {
            return s.Trim().Replace(" ", string.Empty).Replace(Environment.NewLine, string.Empty);
        }
		
		a samo jej wywołanie jest wykorzystywane w klasie ImportedObjectClassMap
		które definiuje sposób mapowania danych pobieranych z pliku 
		
12. sam sposób pobierania danych został również zoptymalizowany w postaci statycznej klsay CsvFileHelper z generyczną metodą   
	public static List<T> GetRecords<T, TClassMap>(string csvFilePath)
            where TClassMap : ClassMap
			
	która to zwraca listę generycznego okreslonego typu, w przypadku naszego użycia ImportedObjectBaseClass
	oraz wymaga okreslenia typu TClassMap który musi dziedziczyć po ClassMap jest to klasa definiująca sposób mapowania danych, w naszym przypadku to ImportedObjectClassMap
	
	taka generyczna metoda może przydac nam siędo wielokrotnego uzywania tego mechanizmu w przypadku pobierania innych, jakichkolwiek opisanych danych z plików csv 

13. ostatnia optymalizacja czyli zmienijszenie złożoności kodu podczas drukowania 
	aktualna logika działa ze złożonością n*n*n (3 pętle foreach w sobie) 
	
	zostało to zoptymalizowane dzięki metodzie .GroupJoin wywołanym dwukrotnie 
	dzięki czemu stworzono taki typ danych do wyświetlania który mozna opisac jako 
	databse który posiada colekcję własnych tabel która to posiada kolekcję własnych column
	
	do wydrukowania również użyto 3 pętli foreach ale ilość iteracji została zmniejszona do iości rzeczywiście wydrukowanych linijek na konsoli czyli 1062
