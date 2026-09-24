{
  pkgs ? import <nixpkgs> { },
}:

with pkgs;

mkShell {
  buildInputs = [
    dotnet-sdk_10
  ];

  shellHook = ''
    unset TEMP TMP TEMPDIR TMPDIR
  '';
}
