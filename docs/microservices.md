# Funcionalidad microservicios

Dentro del folder de [microservicios](../Microservices/) se encontrarán los diferentes microservicios que reemplazarán a la API de AppVecinos.

En este documento puedes encontrar la explicación del flujo de trabajo de diferentes servicios.

# Servicios para nuevos miembros

@startuml

node node1
node node2
node node3
node node4
node node5
node1 -- node2 : label1
node1 .. node3 : label2
node1 ~~ node4 : label3
node1 == node5

@enduml
