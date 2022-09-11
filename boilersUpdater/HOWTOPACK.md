
# How to pack

```
cd boilersUpdater
dotnet clean
dotnet build
dotnet pack boilersUpdater.csproj -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg /p:PackageVersion=1.1.0 -c Release
```