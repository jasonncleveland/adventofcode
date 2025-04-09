This project contains the code used for the [Advent of Code](https://adventofcode.com/) challenge

## C#

The C# dotnet template will need to be installed before it can be used

```bash
# Install the template from the template directory
dotnet new install template/dotnet/
```

The template can now be used to generate a project

```bash
# -n <name> specifies the output folder name created in the current directory
dotnet new daysolution -n <name>
```

Running code using dotnet CLI

```bash
# Run code
dotnet run <data-file>

# Compile code
dotnet publish -c Release

# Run compiled code
./bin/Release/net8.0/linux-x64/build <data-file>
```

## Go

Running code using go CLI
```bash
# Run code
go run . <data-file>

# Run code for single day
go run . <data-file> <dayXX>

# Compile code
go build -o program .

# Run compiled code
./program <data-file>
```

Running tests using go CLI
```bash
# Run tests in directory
go test -coverprofile=coverage.out

# Run tests in all subdirectories
go test ./... -coverprofile=coverage.out && go tool cover -func=coverage.out

# View test coverage
go tool cover -func=coverage.out
```
