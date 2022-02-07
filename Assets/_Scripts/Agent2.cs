using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Agent2 : MonoBehaviour
{
    private NavMeshAgent navMeshAgent; //Variable para hacer referencia al Agente
    public Transform player; //Posicion del player
    public LayerMask whatIsGround, whatIsPlayer; // Capas para saber que es Suelo , y que es jugador

    //Patrullaje
    public Vector3 walkPoint; // Posicion donde el Agente va a patrullar 
    private bool walkPointSet; // Variable donde el Agente tiene que saber SI tiene que buscar otra posicion donde patrullar
    public float walkPointRange; // Maximo y minimo donde se calcularan los puntos por los cuales el Agente pasara

    //Atacar
    public float timeBetweenAttacks; // Tiempo entre Ataques
    bool alreadyAttacked; // SI el jugador ya fue atacado

    //Estados
    [Range(0, 100) , SerializeField]
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



        if(!playerIsInSightRange && !playerIsInAttackRange) // SI mi jugador no esta en el rango de vision ni en el rango de ataque
        {
            Patroling(); // El Agente Patrulla
        }
        if (playerIsInSightRange && !playerIsInAttackRange) //SI mi jugador esta en el rango de vision pero no esta en el rango de ataque
        {
            ChasePlayer(); // El Agente Persigue al jugador
        }
        if(playerIsInSightRange && playerIsInAttackRange) //SI mi jugador esta en el rango de vision y en el rango de ataque
        {
            AttackPlayer(); // El Agente Ataca al jugador
        }

    }
    private void Patroling() // Estado Patrullando
    {
        if (!walkPointSet)
        {
            SearchWalkPoint(); //Buscame un nuevo Punto    
        }
        if (walkPointSet) // SI mi Agente encontro un punto
        {
            navMeshAgent.SetDestination(walkPoint); // LLeva al agente a el destino de ese nuevo punto
           // Vector3 distanceToWalkPoint = transform.position - walkPoint; // Calculo la distancia que esta el Agente de el WalkPoint

            //Llegamos al WalkPoint
            if(navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance) // SI la distancia que esta el Agente es menor a 1 metro de el Walkpoint..
            {
                walkPointSet = false; // walkPointSet es falso
            }
        }
    }
    private void SearchWalkPoint() // Buscar Punto
    {
        //Calcula un punto random del mapa donde se pueda pasar
        float randomZ = Random.Range(-walkPointRange, walkPointRange); // Creo una variable random de el numero de la variable "walkPointRange" que estableci en la cordenada Z
        float randomX = Random.Range(-walkPointRange, walkPointRange); // Creo una variable random de el numero de la variable "walkPointRange" que estableci en la cordenada X

        walkPoint = new Vector3(transform.position.x + randomX,transform.position.y ,transform.position.z + randomZ); //A la variable walkPoint le doy una nueva ubicacion sumando las variables que cree anteriormente y dejando la Y sin modificar

        if(Physics.Raycast(walkPoint, -transform.up , 5f, whatIsGround))// SI mi Punto esta en el mapa..
        {
            walkPointSet = true; //walkPointSet es verdadero
        }
    }
    private void ChasePlayer() // Estado Seguir al jugador
    {
        navMeshAgent.SetDestination(player.position); // El nuevo destino del Agente es la posicion del jugador
    }

    private void AttackPlayer() // Estado Atacar al jugador
    {
        //Asegurarnos que el Agente no se mueva
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
