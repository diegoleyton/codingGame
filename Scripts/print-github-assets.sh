#!/bin/bash

# Obtener el directorio donde está el script
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"

# Subir un nivel → carpeta X
ROOT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"

ASSETS_DIR="$ROOT_DIR/Assets"
BASE_URL="https://github.com/diegoleyton/codingGame/blob/main/Assets"

if [ ! -d "$ASSETS_DIR" ]; then
  echo "Error: No se encontró la carpeta Assets en $ASSETS_DIR"
  exit 1
fi

find "$ASSETS_DIR" -type f ! -name "*.meta" | while read -r file; do
  # ruta relativa dentro de Assets
  rel_path="${file#$ASSETS_DIR/}"

  # encode básico (espacios)
  encoded_path="${rel_path// /%20}"

  echo "$BASE_URL/$encoded_path"
done