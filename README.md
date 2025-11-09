## Instalar dependencias

```bash
dotnet restore
```

## Compilación

Compilar como self-contained

```bash
dotnet publish -c Release -r win-x64 --self-contained true
```

## Encontrar impresora en linux

```bash
lpstat -p

printer POS-80 is idle.  enabled since Sun 09 Nov 2025 02:56:35 PM -03
```

## Ejecutar

```bash
wine escpos.exe "POS-80"
```
