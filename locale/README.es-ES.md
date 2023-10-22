<p align="center">
	<img width="256px" src="https://github.com/GodotParadise/HealthComponent/blob/main/icon.jpg" alt="GodotParadiseHealthComponent logo" />
	<h1 align="center">Godot Paradise Health Component C#</h1>
	
[![LastCommit](https://img.shields.io/github/last-commit/GodotParadise/HealthComponent?cacheSeconds=600)](https://github.com/GodotParadise/HealthComponent/commits)
[![Stars](https://img.shields.io/github/stars/godotparadise/HealthComponent)](https://github.com/GodotParadise/HealthComponent/stargazers)
[![Total downloads](https://img.shields.io/github/downloads/GodotParadise/HealthComponent/total.svg?label=Downloads&logo=github&cacheSeconds=600)](https://github.com/GodotParadise/HealthComponent/releases)
[![License](https://img.shields.io/github/license/GodotParadise/HealthComponent?cacheSeconds=2592000)](https://github.com/GodotParadise/HealthComponent/blob/main/LICENSE.md)
[![Wiki](https://img.shields.io/badge/Read-wiki-cc5490.svg?logo=github)](https://github.com/GodotParadise/HealthComponent/wiki)
</p>

[![es](https://img.shields.io/badge/lang-es-yellow.svg)](https://github.com/GodotParadise/HealthComponent/blob/main/locale/README.es-ES.md)

- - -
Simule sin esfuerzo la salud y el daño de las entidades dentro de tu videojuego ahora en C#.

Este componente maneja todos los aspectos relacionados con la recepción de daños y la gestión de la salud en el nodo padre. Aunque normalmente se añade a un `CharacterBody2D`, no hay limitaciones que impidan su uso con un `StaticRigidBody2D`, lo que le permite imbuir vida a objetos como árboles u otros elementos del juego.

- [Requerimientos](#requerimientos)
- [✨Instalacion](#instalacion)
	- [Automatica (Recomendada)](#automatica-recomendada)
	- [Manual](#manual)
- [Como empezar](#como-empezar)
- [\_Ready()](#_ready)
- [Examples](#examples)
- [Parámetros exportados](#parámetros-exportados)
- [Variables normales accessibles](#variables-normales-accessibles)
- [Funcionalidad](#funcionalidad)
	- [Recibir daño](#recibir-daño)
	- [Curación](#curación)
	- [Curación de vida por segundo](#curación-de-vida-por-segundo)
- [Invulnerabilidad](#invulnerabilidad)
- [Cuando la vida alcanza cero](#cuando-la-vida-alcanza-cero)
	- [Comprobacion manual de muerte](#comprobacion-manual-de-muerte)
- [Porcentaje de la vida actual](#porcentaje-de-la-vida-actual)
- [Multiple health bars](#multiple-health-bars)
- [Señales](#señales)
- [Eres bienvenido a](#eres-bienvenido-a)
- [Normas de contribución](#normas-de-contribución)
- [Contáctanos](#contáctanos)



# Requerimientos
📢 No soportamos versiones inferiores de Godot 3+ ya que nos concentramos en las versiones estables del futuro a partir de la 4 en adelante.
* Godot 4.0+

# ✨Instalacion
## Automatica (Recomendada)
Puedes descargar este plugin desde la [Godot asset library](https://godotengine.org/asset-library/asset/2039) oficial usando la pestaña AssetLib de tu editor Godot. Una vez instalado, estás listo para empezar
## Manual 
Para instalar manualmente el plugin, crea una carpeta **"addons"** en la raíz de tu proyecto Godot y luego descarga el contenido de la carpeta **"addons"** de este repositorio

# Como empezar
Incorpora este componente como nodo hijo en el lugar donde quieras implementar la mecánica de vida y daño. Simplemente define los valores iniciales que deseas asignar a este componente.

![health-component-add](https://github.com/GodotParadise/HealthComponent/blob/main/images/health-component-child-node_add.png)
- - -
![health-component-added](https://github.com/GodotParadise/HealthComponent/blob/main/images/health-component-child-node.png)

# _Ready()
Cuando este componente está listo en el árbol de escena, se llevan a cabo una serie de pasos:

1. Asegurarse de que la salud actual no excede la salud máxima.
2. Configurar el temporizador de regeneración de salud.
3. Configurar el temporizador de invulnerabilidad.
4. Si la regeneración de salud por segundo es superior a cero, activa la regeneración de salud.
5. Establece una conexión con su propia señal `HealthChanged`. Cada vez que cambia la salud, se activa esta señal. Si la regeneración de salud está activada, también se dispara, y si la salud actual llega a cero, se emite una señal `Died`.
6. Establecer una conexión con su propia señal `Died` Una vez que se emite esta señal, se detienen los temporizadores incorporados en el componente.

# Examples
Tenemos una carpeta [ejemplos](https://github.com/GodotParadise/HealthComponent/tree/main/examples) en casi todos nuestros repositorios para mostrar como usar el plugin en un contexto determinado.

En este caso tenemos disponible una barra de progreso simple que esta siendo actualizada segun los cambios que sucedan en el componente de vida.

![health-component-showcase](https://github.com/GodotParadise/HealthComponent/blob/main/images/health_component_showcase.gif)


# Parámetros exportados
- **Max health** *(la vida máxima alcanzable)*
- **Health overflow percentage** *(el porcentaje de vida que puede ser sobrepasado, util para simular mecanicas como escudos)*
- **Current Health** *(la vida actual del nodo)*
- **Health regen** *(la cantida de vida a regenerar segun el tiempo definido por tick)*
- **Health regen tick time** (el tiempo de cada tick para que la cantidad a regenerar sea aplicada)
- **Is Invulnerable** *(si esta activada, no se puede recibir daño pero si curación)*
- **Invulnerability time** *(cuanto va a durar la invulnerabilidad cuando se active, dejar a cero para que sea indefinido)*

# Variables normales accessibles
- **MaxHealthOverflow**
- **enum TYPES {HEALTH, REGEN, DAMAGE}**
- **InvulnerabilityTimer**
- **HealthRegenTimer**

The `MaxHealthOverflow` es una variable computada que representa la suma de la vida maxima con el `health_overflow_percentage` aplicado.

Ejemplo: `max_health of 120 and health overflow percentage of 15% = 138`
Tienes una vida maxima normal de 120 pero puede ser sobrepasada un 15% por lo que el nuevo limite es 138. Esto puede ser util para implementar mecánicas de escudo donde necesitas separar los tipos de vida.

# Funcionalidad
## Recibir daño
Para restar una cantidad específica de salud, puedes invocar sin esfuerzo la función `Damage()` dentro del componente. 
Esto provoca la emisión de una señal `HealthChanged` cada vez que se inflige daño. Además, el componente controla constantemente si la salud actual ha caído en picado hasta cero, activando posteriormente una señal de `Died`.
Cabe destacar que el componente se conecta de forma autónoma a su propia señal de `Died`, lo que detiene simultáneamente el temporizador de regeneración de salud y el temporizador de invulnerabilidad. 

Si la variable `IsInvulnerable` es verdadera, cualquier daño recibido, independientemente de su magnitud, será ignorado. No obstante, se mantendrá la emisión de señales estándar.

```csharp
	private HealthComponent _healthComponent;

	public override void _Ready()
	{
		_healthComponent = GetNode<HealthComponent>("GodotParadiseHealthComponent");
	}

_healthComponent.Damage(10)
_healthComponent.Damage(99)

// Parameter es tratado como un valor absoluto
_healthComponent.Damage(-50) // translate to 50 inside the function
```

## Curación
Similar a la de daño pero esta vez, la cantidad es añadida como vida. Es importante anotar que el proceso de curación cuando la cantidad supera el valor de la variable `MaxHealthOverflow` se usa este como límite.

En cada ejecución de la función, una señal `HealthChanged` es emitida.
```csharp
	private HealthComponent _healthComponent;

	public override void _Ready()
	{
		_healthComponent = GetNode<HealthComponent>("GodotParadiseHealthComponent");
	}

_healthComponent_.Health(25)
// Parameter es tratado como un valor absoluto
_healthComponent.Health(-50) // Se transforma en 50 dentro de la funcion
```
## Curación de vida por segundo
Cuando se invoca la función `Damage()`, la regeneración es activada *(si health_regen es > 0)* hasta que la vida máxima es alcanzada, y en cuanto esto sucede, se desactiva.
Tienes la flexilidad de ajustar la cantidad de curación el intervalo de tiempo en el que tiene que suceder. Dejar el valor de `HealthRegen` a cero si se quiere deshabilitar.

```csharp
	private HealthComponent _healthComponent;

	public override void _Ready()
	{
		_healthComponent = GetNode<HealthComponent>("GodotParadiseHealthComponent");
	}

_healthComponent.EnableHealthRegen(10)
# o deshabilitalo
_healthComponent.EnableHealthRegen(0)
```
# Invulnerabilidad
Tienes la posibilidad de activar o desactivar la invulnerabilidad a través de la función `EnableInvulnerability`. Proporcionando el parámetro enable *(a boolean)*, puedes especificar si la invulnerabilidad está activada o no. Además, puedes establecer un tiempo de duración *(en segundos)* durante el cual la entidad será invulnerable. Una vez alcanzado el límite de tiempo especificado, la invulnerabilidad se desactivará:
```csharp
	private HealthComponent _healthComponent;

	public override void _Ready()
	{
		_healthComponent = GetNode<HealthComponent>("GodotParadiseHealthComponent");
	}

_healthComponent.EnableInvulnerability(true, 2.5)
# Puedes desactivarlo manualmente llamando a la funcion como:
_healthComponent.EnableInvulnerability(false)
```
# Cuando la vida alcanza cero
El componente por si solo emite la señal `Died` ofreciendote la flexibilidad de adaptar el comportamiento que tu juego necesita reaccionando a esta señal. Conectandote a ella puedes ejecutar animaciones, llamar a otras funciones, colectar estadísticas o ejecutar otras acciones relevantes para personalizar la experiencia acorde a los requerimentos de tu juego.

## Comprobacion manual de muerte
Realiza una comprobación manual para determinar si la entidad ha entrado en estado de muerte. Si deseas determinar manualmente este estado, puedes utilizar la función `CheckIsDeath`. Esta función emite la `Died` si la salud actual llega a cero.
```csharp
	private HealthComponent _healthComponent;

	public override void _Ready()
	{
		_healthComponent = GetNode<HealthComponent>("GodotParadiseHealthComponent");
	}

bool isDead = _healthComponent.CheckIsDeath()
```
# Porcentaje de la vida actual
Si desea mostrar una barra de salud, puede acceder al formato del porcentaje de salud a través de la función `GetHealthPercent()`. Esta función devuelve un diccionario estructurado como sigue:
```csharp
// Por ejemplo, este valor indica que tiene un 80% de vida y no hay overflow.

{
   "CurrentHealthPercentage": 0.8,
   "OverflowHealthPercentage": 0.0,
   "OverflowHealth": 0
}

/* De forma similar, considerando una vida maxima de 100 y un overflow de 20% podemos ver que tiene vida maxima y todo el overflow aplicado.
 por lo que su vida real sera 100 + 20
*/
{ "CurrentHealthPercentage": 1.0,
   "OverflowHealthPercentage": 0.2,
   "OverflowHealth": 20
}
```

Esta información puede ayudar a representar con precisión el estado de salud y el desbordamiento en una barra de salud visual.

# Multiple health bars
Para conseguir esta mecánica puedes simplemente añadir múltiples componentes de salud como hijos en el nodo destino y crear una lógica básica de responsabilidad en cadena usando la señal de `Died`. Este es un ejemplo muy básico y te recomendamos que lo adaptes a tus necesidades si son un poco más complejas, sólo queremos darte una idea básica.

```csharp
	private HealthComponent _healthComponent;
	private List<HealthComponent> lifeBars;

	public override void _Ready()
	{
		_healthComponent = GetNode<HealthComponent>("GodotParadiseHealthComponent");
		_healthComponent2 = GetNode<HealthComponent>("GodotParadiseHealthComponent2");
		_healthComponent3 = GetNode<HealthComponent>("GodotParadiseHealthComponent3");

		lifeBars = new {_healthComponent, _healthComponent2, _healthComponent3}
		lifeBars.Last().Died += OnLifeBarConsumed;

	}

	private void OnLifeBarConsumed() {
		HealthComponent lastLifeBar = lifeBars.Last();
		lastLifeBar.Died -= OnLifeBarConsumed; // or queue free this node if you don't need the component anymore

		lifeBars.Remove(lastLifeBar)

		if (lastLifeBar.Count > 0) {
			lifeBars.Last() += OnLifeBarConsumed;
		}
	}

```

# Señales
```csharp
/* 
 Puedes acceder al tipo de action en la señal HealthChanged
 para determinar que tipo de curación / daño se llevo a cabo para reaccionar acorde al flujo de tu juego.
*/

public enum TYPES
{
	DAMAGE,
	HEALTH,
	REGEN
}

[Signal]
public delegate void HealthChangedEventHandler(int amount, int type);
[Signal]
public delegate void InvulnerabilityChangedEventHandler(bool active);
[Signal]
public delegate void DiedEventHandler();
```

# Eres bienvenido a
- [Give feedback](https://github.com/GodotParadise/HealthComponent/pulls)
- [Suggest improvements](https://github.com/GodotParadise/HealthComponent/issues/new?assignees=BananaHolograma&labels=enhancement&template=feature_request.md&title=)
- [Bug report](https://github.com/GodotParadise/HealthComponent/issues/new?assignees=BananaHolograma&labels=bug%2C+task&template=bug_report.md&title=)

GodotParadise esta disponible de forma gratuita.

Si estas agradecido por lo que hacemos, por favor, considera hacer una donación. Desarrollar los plugins y contenidos de GodotParadise requiere una gran cantidad de tiempo y conocimiento, especialmente cuando se trata de Godot. Incluso 1€ es muy apreciado y demuestra que te importa. ¡Muchas Gracias!

- - -
# Normas de contribución
**¡Gracias por tu interes en GodotParadise!**

Para garantizar un proceso de contribución fluido y colaborativo, revise nuestras [directrices de contribución](https://github.com/godotparadise/[PLUGIN]/blob/main/CONTRIBUTING.md) antes de empezar. Estas directrices describen las normas y expectativas que mantenemos en este proyecto.

**Código de conducta:** En este proyecto nos adherimos estrictamente al [Código de conducta de Godot](https://godotengine.org/code-of-conduct/). Como colaborador, es importante respetar y seguir este código para mantener una comunidad positiva e inclusiva.
- - -


# Contáctanos
Si has construido un proyecto, demo, script o algun otro ejemplo usando este plugin haznoslo saber y podemos publicarlo en este repositorio para ayudarnos a mejorar y saber que lo que hacemos es útil.
