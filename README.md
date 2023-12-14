# Wall-E

A un año de la programación de WallE se ha decidido incorporarle una componente para el análisis geométrico.

Mediante un programa, Wall-E será capaz de representar conceptos geométricos (como puntos, líneas o circunferencias), graficarlos y comprobar relaciones que se cumplen (por ejemplo, que las mediatrices en un triángulo se cortan en el mismo punto y que es centro de la circunferencia que lo circunscribe). 

Para ello Wall-E cuenta con un lienzo (plano), una regla y un compás para trazar rectas y circunferencias. Estas herramientas no tienen grabadas las medidas por lo que WallE nunca sabe la distancia real entre dos puntos.

## GSharp

Para especificar las acciones que deberá realizar Wall-E para su análisis geométrico se utilizará un lenguaje de programación denominado G#. El lenguaje G# es un lenguaje funcional que no permite variables. Un programa en G# se compone de una lista de instrucciones separadas por `;`. 

Las instrucciones permiten recibir argumentos de entrada, importar otros códigos, definir funciones o constantes, configurar características del visor y dibujar objetos  geométricos.

## Intrucciones para ejecutar el proyecto

Para ejecutar el proyecto usted debe tener instalado lo siguiente en su computadora.
- `.NET 7.0` puede seguir las [instrucciones de instalación de Microsoft](https://learn.microsoft.com/en-us/dotnet/core/install/).
- `Godot 4.1.2` puede descargarlo en la [página oficial de Godot](https://godotengine.org/).

Después de tener instalado lo anterior en su computadora debe ser capaz de ejecutar el proyecto con el siguiente comando:

```
dotnet run & godot-engine
```

Para la evaluación del proyecto también se le puede proveer al profesor un ejecutable de la aplicación para Mac, Windows o Linux, para ello contáctenos.

## Funcionalidades de GSharp

Un programa en G# es un conjunto de instrucciones. El conjunto de instrucciones vacío es un programa válido y no hace nada. Las instrucciones de entrada permiten declarar argumentos que serán provistos por la aplicación.

No pueden existir dos argumentos con igual nombre y de distintos tipos. Los argumentos pueden requerirse en cualquier lugar del código pero serán provistos por la aplicación antes de la ejecución, sin importar si la ejecución termina requiriéndolos finalmente o no (por ejemplo, están dentro de una función que no se evalúa nunca). Un argumento se puede pedir varias veces en un mismo ámbito, pero no se puede referir si no está declarado en el ámbito actual o en algún ámbito padre.

### Secuencias 

El concepto de secuencia está presente en el lenguaje G#. Los valores dentro de la secuencias deben ser valores del mismo tipo. Algunas funciones intrínsecas del lenguaje como `intersect` devuelven una secuencia de valores (en este caso de 
puntos). 

Para consultar los valores que están contenidos en una secuencia se utiliza la declaración de match:

```
valor0, valor1, ..., resto = secuencia;
```

Esto asigna a las primeras constantes los primeros valores de la secuencia y en la última constante la secuencia de los valores restantes. Si no se desea trabajar con el resto se puede utilizar el comodín underscore `_` tantas veces como desee en G#.

Si se pide valores a una secuencia que no tiene, las constantes declaradas en la asignación obtienen valor `undefined`.

Dada una secuencia se puede conocer cuántos elementostiene utilizando la función intrínseca `count`. Si la secuencia es infinita (`undefined` o realmente infinita) esta función evalúa `undefined`.

#### Secuencias Infinitas

Si se desea un rango de valores enteros se puede utilizar la notación: `{a ... b}` donde `a` y `b` son valores constantes enteros. Si se desea empezar en `a` y obtener todos los valores sucesivos se puede escribir `{a ... }`.

-----------

Además como una funcionalidad adicional usted puede combinar la declaración de secuencias enteras con rangos de valores enteros. O sea, usted podrá declarar secuencia como `{1, 2, 3, 4 ... 9, 5 ... 10, 11, 12, 20 ...}`

-----------

Otras secuencias infinitas se pueden obtener de las funciones intrínsecas:

```
points(f) // devuelve una secuencia de puntos aleatorios en una figura 
samples() // devuelve una secuencia de puntos aleatorios en el lienzo 
randoms() // devuelve una secuencia de valores aleatorios enteros positivos
```

### Valores numéricos 

Las expresiones numéricas representan valores básicos en G#. Entre estos se pueden utilizar operadores aritméticos (`+`, `-`, `*`, `/`, `%`) y de comparación (`<`, `<=`, `>`, `>=`, `==`, `!=`) para definir expresiones más complejas.

### Haciendo mediciones

Wall-E no conoce las coordenadas de un punto, pero puede utilizar el compás para señalar una medida entre dos puntos. Esta medida puede utilizarse para trazar una circunferencia en cualquier posición, para comparar con otra medida (si se tiene que abrir más o menos el compás para encajar con otro par de puntos).

El objeto medida (`measure`) se puede crear a partir de dos puntos. 

Dadas dos medidas también se puede obtener una medida que sea la suma (basta con trazar una recta y ubicar dos circunferencias, primero una con radio igual a la primera medida, y luego otra ubicada en una de las intersecciones con radio igual a la segunda). La diferencia entre dos medida es también posible (siendo siempre “positiva”). Son válidas también las operaciones siguientes:

```
measure * natural = measure // qué medida se obtiene replicando un número de veces una medida
measure / measure = natural // cuántas veces cabe una medida dentro de otra
```

### Condicionales

La expresión

```
if <expr> then <expr> else <expr>
```

Permite expresar valores condicionales. La expresión de la cláusula `if` se evalúa. Si es "verdadera" la expresión completa tiene el valor de la cláusula `then`, si es "falsa" tiene el valor de la cláusula `else`.

En G# no existen los valores booleanos pero en su lugar se puede utilizar cualquier objeto o número. Las expresiones que evalúan "falso" son las siguientes:

* `0` valor numérico
* `{}` secuencia vacía
* `undefined` valor de un objeto que no está bien definido

Cualquier otro valor evalúa verdadero. Las expresiones pueden ser operadas con `and` (`&`), `or` (`|`) y `not` (`!`) devolviendo `1` en caso de evaluar verdadero y `0` en caso de evaluar falso.

### Encapsulando con funciones

En G# se pueden definir funciones de la siguiente forma:

```
<identificador>(<lista de parámetros>) = <expresión>;
```

Como el cuerpo de una función se forma de una única expresión se necesita un tipo de expresión que permita tener varias acciones antes de evaluar la expresión final.

----------

Adicionalmente, usted podrá definir el tipo definido de cada parámetro y el tipo de retorno de la función, usando `:` después del nombre del paramétro que usted desea. Los tipos soportados son:

* point
* line
* ray
* segment
* circle
* arc
* scalar
* measure
* string

Por ejemplo, usted podrá declarar alternativamente la función Fibonacci de la siguiente forma:

```
Fib(n: scalar) : scalar = if n <= 1 then 1 else Fib(n – 1) + Fib(n – 2);
```

----------

### Expresión Let-In

La expresión `let-in` tiene la siguiente sintáxis:

```
let
<lista de instrucciones terminadas con ;> 
in <expresión>
```

Las constantes no pueden redefinirse en ambientes anidados para que no parezca que se les está pudiendo cambiar el valor. En un `let` anidado se puede utilizar cualquier constante declarada en un `let` “padre” excepto aquella a la que se le está siendo asignada la expresión con el let involucrado. Es decir, el siguiente código no es válido.

### Chequeo de tipos

El lenguaje G# tiene un fuerte chequeo de tipos y debe descubrir tempranamente (en compilación), cualquier posible conflicto con los tipos de sus expresiones y las operaciones que pueden hacerse sobre estas. Por ejemplo, no tiene sentido interceptar dos números, o sumar dos circunferencias (al menos no en este lenguaje).

Para ello en el lenguaje se debe realizar una inferencia de los tipos de las expresiones. A diferencia de otros lenguajes en los que las variables, parámetros y retornos de función tienen declarado los tipos, en G# solo se conoce los tipos de los argumentos de la aplicación (`point`, `line`, `segment`, etc.) y de las funciones intrínsecas (`intersect`, `count`, `randoms`, etc.).

### Bibliotecas

Desde un programa de puede importar las instrucciones declaradas en otros ficheros mediante la instrucción:

```
import “NombreDeFichero.geo”
```

Este tipo de instrucción puede aparecer en cualquier lugar del programa, en caso de que hayan errores en dicha biblioteca, se notificará al usuario con un rastro de las importaciones para que pueda ver dónde fue. Si una biblioteca importada tiene definiciones repetidas, se produce un error de compilación. Si se importan dos veces un mismo fichero, este deberá ser incluido una única vez (la primera).

### Adicionales

Adicionalmente, en este proyecto podrá usar `while` y `for` para iterar. El operador `[]` para indexar secuencias, cadenas de caracteres, o acceder a la coordenada x (`[0]`) o y (`[1]`) de un punto. El operador `:=` para modificar valores
de las variables, y los operadores `++`, `--`, `+=`, `-=` para operar y asignar más fácilmente.