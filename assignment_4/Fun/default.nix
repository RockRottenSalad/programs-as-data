{ pkgs ? import <nixpkgs> {} }:
pkgs.mkShell {
    buildInputs = with pkgs; [
        dotnet-sdk_10
        fsautocomplete
        nuget
        fsharp
        fantomas
    ];
}

