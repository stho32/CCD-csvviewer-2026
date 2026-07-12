#!/usr/bin/env bash
# Experimenteller Testlauf des CsvViewers mit der persistierten beispiel.csv.
# Aufruf: ./testlauf.sh [seitengröße]   (Default: 5 — bei 18 Datenzeilen ergibt das 4 Seiten)
set -euo pipefail
cd "$(dirname "$0")"

SEITENGROESSE="${1:-5}"

dotnet build Source/CsvViewer/CsvViewer.sln --nologo --verbosity quiet
dotnet run --no-build --project Source/CsvViewer/CsvViewer/CsvViewer.csproj -- beispiel.csv "$SEITENGROESSE"
