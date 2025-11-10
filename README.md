# ESC/POS Wrapper

Un wrapper en C# para imprimir en impresoras térmicas ESC/POS mediante comandos JSON. Utiliza la librería [ESC-POS-USB-NET](https://github.com/mtmsuhail/ESC-POS-USB-NET) y funciona en Linux usando Wine.

## 🚀 Instalación y Configuración

### Prerrequisitos

- .NET 9.0 SDK
- Wine (para ejecutar en Linux)
- Impresora térmica ESC/POS

### Compilación

1. **Restaurar dependencias:**
```bash
dotnet restore
```

2. **Compilar como self-contained:**
```bash
dotnet publish -c Release -r win-x64 --self-contained true
```

3. **Ubicación del ejecutable:**
```
bin/Release/net9.0/win-x64/publish/escpos.exe
```

## 🖨️ Configuración de Impresora

### Encontrar impresoras disponibles en Linux:

```bash
lpstat -p
```

Ejemplo de salida:
```
printer POS-80 is idle.  enabled since Sun 09 Nov 2025 02:56:35 PM -03
```

## 📝 Uso

### Sintaxis básica:
```bash
wine escpos.exe "NOMBRE_IMPRESORA" '[COMANDOS_JSON]'
```

### Ejemplo simple:
```bash
wine escpos.exe "POS-80" '[{"Type":"append","Text":"Hola Mundo!"},{"Type":"fullpapercut"}]'
```

## 📚 Comandos Disponibles

### 📝 **TEXTO**

#### `append`
Agrega texto con salto de línea
```json
{"Type":"append","Text":"Hola Mundo!"}
```

#### `appendwithoutlf`
Agrega texto sin salto de línea
```json
{"Type":"appendwithoutlf","Text":"Texto"}
```

### 🔤 **MODOS DE FUENTE**

#### `boldmode`
Modo negrita (con texto o estado)
```json
{"Type":"boldmode","Text":"Texto en negrita"}
{"Type":"boldmode","State":true}
```

#### `underlinemode`
Modo subrayado
```json
{"Type":"underlinemode","Text":"Texto subrayado"}
{"Type":"underlinemode","State":true}
```

#### `expandedmode`
Modo expandido
```json
{"Type":"expandedmode","State":true}
```

#### `condensedmode`
Modo condensado
```json
{"Type":"condensedmode","State":true}
```

### 📐 **ANCHO DE FUENTE**

#### `doublewidth2` / `doublewidth3` / `normalwidth`
Control del ancho de fuente
```json
{"Type":"doublewidth2"}
{"Type":"doublewidth3"}
{"Type":"normalwidth"}
```

### 📍 **ALINEACIÓN**

#### `alignleft` / `alignright` / `aligncenter`
```json
{"Type":"aligncenter"}
```

### 🎨 **FUENTES**

#### `font`
Cambiar tipo de fuente
```json
{"Type":"font","Text":"Fuente A","FontType":0}
```

**Tipos de fuente disponibles:**
- `FontA = 0`
- `FontB = 1`
- `FontC = 2`
- `FontD = 3`
- `FontE = 4`
- `SpecialFontA = 5`
- `SpecialFontB = 6`

### ➖ **SEPARADORES**

#### `separator`
```json
{"Type":"separator"}
{"Type":"separator","Character":"="}
```

### 📏 **ALTURA DE LÍNEA**

#### `setlineheight` / `normallineheight`
```json
{"Type":"setlineheight","Height":40}
{"Type":"normallineheight"}
```

### ⏎ **SALTOS DE LÍNEA**

#### `newline` / `newlines`
```json
{"Type":"newline"}
{"Type":"newlines","Lines":3}
```

### ✂️ **CORTE DE PAPEL**

#### `fullpapercut` / `partialpapercut`
```json
{"Type":"fullpapercut"}
{"Type":"partialpapercut"}
```

### 💰 **CAJÓN**

#### `opendrawer`
```json
{"Type":"opendrawer"}
```

### 📊 **CÓDIGOS DE BARRAS**

#### `code128` / `code39` / `ean13`
```json
{"Type":"code128","Text":"123456789","Position":2}
{"Type":"code39","Text":"123456789","Position":1}
{"Type":"ean13","Text":"1234567891231","Position":0}
```

**Posiciones disponibles:**
- `NotPrint = 0` (no imprimir texto)
- `AboveBarcode = 1` (arriba del código)
- `BelowBarcode = 2` (abajo del código)
- `Both = 3` (arriba y abajo)

### 📱 **CÓDIGOS QR**

#### `qrcode`
```json
{"Type":"qrcode","Text":"https://example.com","QrSize":2}
```

**Tamaños disponibles:** `0` a `16` (0 = más pequeño, 16 = más grande)

### 🖼️ **IMÁGENES**

#### `image`
```json
{"Type":"image","ImagePath":"/ruta/completa/imagen.png"}
```

**Formatos soportados:** BMP, PNG, JPG

### 🔧 **UTILIDADES**

#### `clear` / `initializeprint` / `autotest` / `testprinter`
```json
{"Type":"clear"}
{"Type":"initializeprint"}
{"Type":"autotest"}
{"Type":"testprinter"}
```

## 🎯 Ejemplos Completos

### Factura Simple
```bash
wine escpos.exe "POS-80" '[
  {"Type":"initializeprint"},
  {"Type":"aligncenter"},
  {"Type":"boldmode","Text":"FACTURA ELECTRÓNICA"},
  {"Type":"normalwidth"},
  {"Type":"separator","Character":"="},
  {"Type":"alignleft"},
  {"Type":"append","Text":"Cliente: Juan Pérez"},
  {"Type":"append","Text":"Fecha: 09/11/2025"},
  {"Type":"append","Text":"Total: $150.00"},
  {"Type":"separator"},
  {"Type":"aligncenter"},
  {"Type":"qrcode","Text":"FAC-001-2025","QrSize":1},
  {"Type":"fullpapercut"}
]'
```

### Código de Barras con Texto
```bash
wine escpos.exe "POS-80" '[
  {"Type":"aligncenter"},
  {"Type":"append","Text":"Producto: ABC123"},
  {"Type":"code128","Text":"ABC123456","Position":2},
  {"Type":"separator"},
  {"Type":"fullpapercut"}
]'
```

### Recibo con Logo
```bash
wine escpos.exe "POS-80" '[
  {"Type":"aligncenter"},
  {"Type":"image","ImagePath":"/home/usuario/logo.png"},
  {"Type":"boldmode","Text":"MI EMPRESA"},
  {"Type":"normalwidth"},
  {"Type":"append","Text":"Gracias por su compra"},
  {"Type":"opendrawer"},
  {"Type":"fullpapercut"}
]'
```

### Test de Fuentes y Estilos
```bash
wine escpos.exe "POS-80" '[
  {"Type":"append","Text":"Texto normal"},
  {"Type":"boldmode","Text":"Texto en negrita"},
  {"Type":"underlinemode","Text":"Texto subrayado"},
  {"Type":"expandedmode","State":true},
  {"Type":"append","Text":"Texto expandido"},
  {"Type":"expandedmode","State":false},
  {"Type":"doublewidth2"},
  {"Type":"append","Text":"Ancho doble"},
  {"Type":"normalwidth"},
  {"Type":"fullpapercut"}
]'
```

## 🛠️ Troubleshooting

### Problemas comunes:

1. **Error "Unable to access printer":**
   - Verificar que la impresora esté encendida
   - Confirmar el nombre exacto con `lpstat -p`

2. **Caracteres especiales no se imprimen bien:**
   - El wrapper usa codificación CP850 para caracteres españoles

3. **Wine no encuentra el ejecutable:**
   - Usar ruta absoluta al ejecutable
   - Verificar permisos de ejecución

## 📄 Licencia

Este proyecto utiliza la librería [ESC-POS-USB-NET](https://github.com/mtmsuhail/ESC-POS-USB-NET) bajo licencia MIT.

## 🤝 Contribuciones

Las contribuciones son bienvenidas. Por favor abre un issue antes de enviar pull requests.
