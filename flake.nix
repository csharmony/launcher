{
  description = "Launcher";

  inputs = {
    nixpkgs.url = "github:nixos/nixpkgs?ref=nixpkgs-unstable";
  };

  outputs =
    { nixpkgs, ... }:
    let
      pkgs = nixpkgs.legacyPackages.x86_64-linux;
    in
    {
      devShells.x86_64-linux.default = pkgs.mkShell {
        packages = with pkgs; [
          dotnet-sdk_10

          # for AOT builds
          clang
          zlib
          brotli
        ];

        # for running Avalonia
        LD_LIBRARY_PATH = pkgs.lib.makeLibraryPath (
          with pkgs;
          [
            # common
            fontconfig

            # x11
            libx11
            libice
            libsm
            libxcursor
            libxrandr
            libxi
            libxext
            libxfixes

            # wayland
            wayland
            libxkbcommon
            libgbm
            libdrm

            # gpu acceleration
            libglvnd
            mesa
          ]
        );
      };
    };
}
