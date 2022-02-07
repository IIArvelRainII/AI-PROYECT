using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Agent3 : MonoBehaviour
{
    private NavMeshAgent navMeshAgent; //Variable para hacer referencia al Agente
    public Transform player; //Posicion del player
    public LayerMask whatIsGround, whatIsPlayer; // Capas para saber que es Suelo , y que es jugador

    //Patrullaje
    public Transform[] wayPoints;
    private int currentWayPointIndex;

    //Atacar
    public float timeBetweenAttacks; // Tiempo entre balas
    bool alreadyAttacked; // SI el jugador ya fue atacado

    //Estados
    [Range(0, 100), SerializeField]
    public float sightRange, attackRange; // Rango de vision , Rango de ataque
    public bool playerIsInSightRange, playerIsInAttackRange; // SI el jugador esta en el rango de vision , SI el jugador esta en rango de ataque 

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>().transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        //Variables de Chequeo para ver si el jugador esta en la zona de vision o de ataque
        playerIsInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer); // Creo una variable para definir el rango de deteccion de vision del agente (Mi posicion, el rango que tiene la esfera de vision , Si es el jugador )
        playerIsInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer); // Creo una variable para definir el rango de deteccion de ataque del agente (Mi posicion , el rango de la esfera de ataque , Si es el jugador)

        if (!playerIsInSightRange && !playerIsInAttackRange) // SI mi jugador no esta en el rango de vision ni en el rango de ataque
        {
            Patroling(); // El Agente Patrulla
        }
        if (playerIsInSightRange && !playerIsInAttackRange) //SI mi jugador esta en el rango de vision pero no esta en el rango de ataque
        {
            ChasePlayer(); // El Agente Persigue al jugador
        }
        if (playerIsInSightRange && playerIsInAttackRange) //SI mi jugador esta en el rango de vision y en el rango de ataque
        {
            AttackPlayer(); // El Agente Ataca al jugador
        }

    }
    private void Patroling() // Estado Patrullando
    {
        if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)//SI la distancia del Agente es menor a la distancia en el cual el Agente se tiene que frenar
        {
            currentWayPointIndex = (currentWayPointIndex + 1) % wayPoints.Length;//Nuestra actual posicion la pasamos a la siguiente posicion , el operador "%" sirve para que cuando llegemos al ultimo destino se repita el ciclo ejemplo: 01234..01234
            navMeshAgent.SetDestination(wayPoints[currentWayPointIndex].position);//Dale al agente su nuevo destino
        }
    }
   
    private void ChasePlayer() // Estado Seguir al jugador
    {
        navMeshAgent.SetDestination(player.position); // El nuevo destino del Agente es la posicion del jugador
    }

    private void AttackPlayer() // Estado Atacar al jugador
    {
        //Asegurarnos que el jugador no se mueva
        navMeshAgent.SetDestination(transform.position); // El nuevo destino del Agente es su propia posicion lo que genera que el agente no se mueva

        transform.LookAt(player); //Le decimos al Agente que mire al jugador    

        if (!alreadyAttacked) // SI NO ataque al jugador.. 
        {
            ///El Codigo de ataque va Aqui


            ///

            alreadyAttacked = true; // Ya lo ataque
            Invoke(nameof(ResetAttack), timeBetweenAttacks); // LLama a la funcion ResetAttack con un delay que podemos modificar en la variable "timeBetweenShots"
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;//No lo atacaste
    }

    private void OnDrawGizmos() // Aca pintamos
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
