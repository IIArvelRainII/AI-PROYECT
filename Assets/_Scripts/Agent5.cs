using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Agent5 : MonoBehaviour
{
    private NavMeshAgent navMeshAgent; //Variable para hacer referencia al Agente
    public float radius; //Valor del radio
    [Range(0, 360), SerializeField]
    public float angle; //Valor del Angulo 
    public float attackRange; // Rango de ataque
    public GameObject player; // Referencia al GameObject del Player
    private PlayerController playerScript;
    public Transform playerTransform; // Referencia al transform del Player
    public LayerMask whatIsGround, whatIsPlayer; // Capas para saber que es Suelo , y que es Player
    public bool canSeePlayer, playerIsInAttackRange, ialreadyBotherHim ; // Un check booleano (Si podemos o No ver al Player)

    //Patrullaje
    public Vector3 walkPoint; // Posicion donde el Agente va a patrullar 
    private bool walkPointSet; // Variable donde el Agente tiene que saber SI tiene que buscar otra posicion donde patrullar
    public float walkPointRange; // Maximo y minimo donde se calcularan los puntos por los cuales el Agente pasara

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<PlayerController>().gameObject;
        playerScript = FindObjectOfType<PlayerController>();
        playerTransform = FindObjectOfType<PlayerController>().transform;
        StartCoroutine(FOVRoutine());
    }

    private void Update()
    {

    }

    private IEnumerator FOVRoutine() // Usamos una corrutina para que el agente no tenga que calcular a cada frame donde esta el Player
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f); // Lo que demora en empezar a calcular la ubicacion del player
        while (true) // Mientras sea verdadero
        {
            yield return wait; // espera el delay para repetir
            FieldOfViewCheck(); // LLama a el metodo "FieldOfViewCheck"
        }
    }

    private void FieldOfViewCheck() //Campo de vision de chequeo
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, whatIsPlayer); // Creamos una Arrey de colliders en el cual lo unico que nos importa que nos de es el collider del jugador (Mi posicion , Radio , Capa donde se encuentra el Player)  
        playerIsInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer); // Creo una variable para definir el rango de deteccion de ataque del agente (Mi posicion , el rango de la esfera de ataque , Si es el Player)

        if (rangeChecks.Length != 0) // SI nuestro rangeChecks encontro algo
        {
            Transform Player = rangeChecks[0].transform; // Danos el transform de lo primero que encontraste en la capa "WhatIsPlayer" que en este caso tendria que ser el Player
            Vector3 directionToPlayer = (Player.position - transform.position).normalized; // Saber en que direccion esta mirando el Agente respecto al Player

            if (Vector3.Angle(transform.forward, directionToPlayer) < angle / 2) // SI mi angulo desde mi Origen hasta el jugador es menor al angulo dividido 2
            {
                float distanceToPlayer = Vector3.Distance(transform.position, Player.position); //Dame la cantidad de distancia que esta el Agente del Player

                if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, whatIsGround) && ialreadyBotherHim == false)//SI NO estamos detectando algo que este en la Capa de whatIsGround entonces debemos estar detectando al Player (Nuestra posicion , Direccion hacia el Player , La distancia que esta el Agente al Player , Frenamos el rayCast si choca con algo que este en la capa whatIsGround)
                {
                    canSeePlayer = true; // Podemos ver al Player
                    if (canSeePlayer) // SI vemos al Player 
                    {
                        navMeshAgent.SetDestination(playerTransform.position); // Dame como nuevo destino del Agente la posicion del Player
                        if (playerIsInAttackRange && ialreadyBotherHim == false) // SI Player esta en el rango de ataque
                        {
                            ///El Codigo de ataque va Aqui
                            ///
                            /// 
                            /// 
                            ///
                            navMeshAgent.SetDestination(transform.position);

                            ialreadyBotherHim = true;
                            canSeePlayer = false;
                        }
                    }
                }
                else
                {
                    canSeePlayer = false; // No podemos ver al Player
                }

            }
            else // SI no esta en nuestro angulo entonces no lo vemos
            {
                canSeePlayer = false; // No podemos ver al Player
            }
        }
        else if (canSeePlayer)//SI nuestro Player paso por la deteccion.. pero despues se salio..
        {
            canSeePlayer = false; // Ya no podemos ver al Player
        }


        if (canSeePlayer == false) // SI no podemos ver a nuestro Player
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
                if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance) // SI la distancia que esta el Agente es menor a 1 metro de el Walkpoint..
                {
                    walkPointSet = false; // walkPointSet es falso
                    ialreadyBotherHim = false;
                }
            }
        }


    }
    private void SearchWalkPoint()
    {
        //Calcula un punto random del mapa donde se pueda pasar
        float randomZ = Random.Range(-walkPointRange, walkPointRange); // Creo una variable random de el numero de la variable "walkPointRange" que estableci en la cordenada Z
        float randomX = Random.Range(-walkPointRange, walkPointRange); // Creo una variable random de el numero de la variable "walkPointRange" que estableci en la cordenada X

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ); //A la variable walkPoint le doy una nueva ubicacion sumando las variables que cree anteriormente y dejando la Y sin modificar

        if (Physics.Raycast(walkPoint, -transform.up, 5f, whatIsGround))// SI mi Punto esta en el mapa..
        {
            walkPointSet = true; //walkPointSet es verdadero
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

