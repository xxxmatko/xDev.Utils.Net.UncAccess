# xDev.Utils.Net.UncAccess

Utilita pre prístup k adresárom a súborom v rámci siete.

## Použitie

Pre použitie a získanie prístupu k adresárom alebo súborom v rámci siete, treba
vytvoriť inštanciu triedy ```UncAccess``` a zadať UNC cestu, ku ktorej chceme
pristupovať, a taktiež meno a heslo užívateľa pod ktorým chceme pristupovať k 
danému zdroju.

```csharp
// Vytvorime instanciu triedy UncAccess
using (UncAccess unc = new UncAccess(@"\\localhost\UncTemp", null, "user", "user123"))
{
	// Skontrolujeme ci sa podarilo ziskat pristup k UNC ceste
	if(unc.LastError != 0)
	{
		Console.WriteLine("Failed to connect to UNC path, LastError = " + unc.LastError);
		return;
	}

	// Pokusime sa vytvorit subor na danej ceste
	var file = new FileInfo(@"\\localhost\UncTemp\test.txt");
	using (FileStream fs = file.Create())
	{
		Byte[] info = new UTF8Encoding(true).GetBytes("This is some text in the file.");

		//Add some information to the file.
		fs.Write(info, 0, info.Length);
	}
}
```
