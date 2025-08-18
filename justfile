set positional-arguments

build:
    dotnet build

run:
    dotnet run --project BarclaysCodingTest/BarclaysCodingTest.csproj

test:
    dotnet test BarclaysCodingTest.Test/BarclaysCodingTest.Test.csproj

@ef *args='':
    dotnet ef --project BarclaysCodingTest/BarclaysCodingTest.csproj $@
