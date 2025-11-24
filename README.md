# ProyectoFinalParadigmas
Proyecto Final Paradigmas

# Proyecto Final – Parte 1: Diseño de Arquitectura
### Paradigmas y Técnicas de Programación
### RoboEscape: The Lab Simulation

**Integrantes:**
- Irene Turrado Sierra  
- María Román Cantillana

## Descripción del proyecto
RoboEscape: The Lab Simulation es un juego de sigilo donde el jugador controla un robot que intenta escapar de un laboratorio vigilado por drones con diferentes comportamientos.  
El diseño aplica principios SOLID y patrones de diseño como Singleton, State, Observer, Factory, Strategy y Component.

Esta entrega corresponde a la Primera Parte: Diseño de Arquitectura.

## Contenido del repositorio

### 1. Diagrama UML
Incluye todos los módulos principales del sistema:
- IA de drones y máquina de estados  
- Sistema de ruido  
- Sistema de dificultad  
- GameManager y flujo de partida  
- UI Manager, puntuación y sonido  

Archivo:  
UMLProyecto.drawio.html

### 2. Documento explicativo
Memoria del diseño de arquitectura, con historias de usuario, descripción de componentes, patrones aplicados y decisiones de diseño.

Archivo:  
ProyectoFinalDoc.docx

## Arquitectura y patrones principales
- Singleton: GameManager centraliza la lógica global del juego.  
- State: Los drones alternan entre Patrol, Chase, Search y GlobalAlert.  
- Factory: EnemyFactory crea drones según configuración.  
- Observer: Comunicación entre GameManager, UIManager, ScoreSystem y sonido.  
- Strategy: Sistema de dificultad extensible (Easy, Normal, Hard).  
- Component: Modularización del jugador (movimiento, interacción, ruido).

## Ciclo de juego
Menú → Partida → Victoria/Derrota → Reinicio o volver al menú.  
El jugador elige escenario y dificultad, lo que afecta el comportamiento de la IA y la puntuación.

## Entrega
Se genera un archivo .zip con el contenido del repositorio para subirlo a Aula Global, siguiendo las instrucciones de la asignatura.
