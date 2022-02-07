using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Agent : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;

    public Transform[] wayPoints; // Hacemos una Array de todos los puntos en los que va a pasar el agente

    private int currentWayPointIndex; // Creamos una variable entero para que determine el WayPoint en el que esta llendo el Agente actualmente, el index empieza en 0 que seria la primera posicion
    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.SetDestination(wayPoints[currentWayPointIndex].position); // Al inicio del primer frame dale el primer destino al agente en este caso es la primera posicion ya que el index estaria en "0"
    }                  

    // Update is called once per frame
    void Update()
    {
        if(navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance) //SI la distancia del Agente es menor a la distancia en el cual el Agente se tiene que frenar
        {
            currentWayPointIndex = (currentWayPointIndex + 1) % wayPoints.Length; //Nuestra actual posicion la pasamos a la siguiente posicion , el operador "%" sirve para que cuando llegemos al ultimo destino se repita el ciclo ejemplo: 01234..01234
            navMeshAgent.SetDestination(wayPoints[currentWayPointIndex].position);//Dale al agente su nuevo destino
        }
    }
}
