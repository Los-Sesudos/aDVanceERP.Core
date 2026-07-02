# aDVance ERP - Core

[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)
[![Language](https://img.shields.io/badge/Language-C%23-239120?logo=csharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)

Núcleo del sistema **aDVance ERP**, una plataforma empresarial modular y extensible construida en C# que proporciona modelos, servicios, repositorios y controladores, así como interfaces y clases base para extensiones modulares.

## 📋 Descripción

**aDVanceERP.Core** es el fundamento arquitectónico del sistema aDVance ERP. Proporciona:

- **Modelos de datos**: Estructuras base para entidades empresariales
- **Servicios**: Lógica de negocio reutilizable
- **Repositorios**: Acceso a datos con patrón Repository
- **Controladores**: Endpoints de API REST
- **Extensiones modulares**: Interfaces y clases base para crear módulos personalizados

## 🚀 Características

- ✅ Arquitectura modular y escalable
- ✅ Patrón de repositorio para acceso a datos
- ✅ Interfaces extensibles para módulos personalizados
- ✅ Licencia GPL v3 (software libre)
- ✅ Construido con .NET/C#

## 📦 Requisitos

- .NET 6.0 o superior
- Visual Studio 2022 o Visual Studio Code
- Git

## 🔧 Instalación

### Clonar el repositorio

```bash
git clone https://github.com/Los-Sesudos/aDVanceERP.Core.git
cd aDVanceERP.Core
```

### Restaurar dependencias

```bash
dotnet restore
```

### Compilar

```bash
dotnet build
```

## 📚 Estructura del Proyecto

```
aDVanceERP.Core/
├── Models/           # Modelos de datos
├── Services/         # Lógica de negocio
├── Repositories/     # Acceso a datos
├── Controllers/      # Endpoints API
├── Interfaces/       # Contratos para extensiones
└── Base/             # Clases base para módulos
```

## 🔌 Extensiones Modulares

Este proyecto está diseñado para ser extensible. Puedes crear módulos personalizados implementando las interfaces base proporcionadas:

```csharp
public class MiModuloCustomizado : IModuloBase
{
    // Implementación de tu lógica
}
```

## 📖 Documentación

Para más información sobre cómo desarrollar módulos y extender el sistema, consulta la documentación del proyecto.

## 🤝 Contribuciones

Las contribuciones son bienvenidas. Para contribuir:

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📝 Licencia

Este proyecto está bajo la licencia **GNU General Public License v3.0** - ver el archivo [LICENSE](LICENSE) para más detalles.

## 👥 Autores

Desarrollado por **Los Sesudos** - [GitHub Organization](https://github.com/Los-Sesudos)

## 📧 Soporte

Para reportar bugs, sugerencias o preguntas, utiliza la sección de [Issues](https://github.com/Los-Sesudos/aDVanceERP.Core/issues).

---

**¡Gracias por ser parte de aDVance ERP!** 🎉
