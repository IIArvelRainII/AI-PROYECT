using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Agent4 : MonoBehaviour
{
    private NavMeshAgent navMeshAgent; //Variable para hacer referencia al Agente
    public float radius; //Valor del radio
    [Range(0,360) , SerializeField]
    public float angle; //Valor del Angulo 
    public float attackRange; // Rango de ataque
    public GameObject player; // Referencia al GameObject del Player
    private PlayerController playerScript;
    public Transform playerTransform; // Referencia al transform del Player
    public Transform[] wayPoints; // Hacemos una Array de todos los puntos en los que va a pasar el agente
    private int currentWayPointIndex; // Creamos una variable entero para que determine el WayPoint en el que esta llendo el Agente actualmente, el index empieza en 0 que seria la primera posicion
    public LayerMask whatIsGround, whatIsPlayer; // Capas para saber que es Suelo , y que es Player
    public bool canSeePlayer , playerIsInAttackRange , rage; // Un check booleano (Si podemos o No ver al Player) , Modo " Rage "(ira)

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

            if(Vector3.Angle(transform.forward , directionToPlayer) < angle / 2) // SI mi angulo desde mi Origen hasta el jugador es menor al angulo dividido 2
            {
                float distanceToPlayer = Vector3.Distance(transform.position, Player.position); //Dame la cantidad de distancia que esta el Agente del Player

                if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, whatIsGround) && playerScript.moveSpeed >= 10 || rage == true)//SI NO estamos detectando algo que este en la Capa de whatIsGround entonces debemos estar detectando al Player (Nuestra posicion , Direccion hacia el Player , La distancia que esta el Agente al Player , Frenamos el rayCast si choca con algo que este en la capa whatIsGround) Y el Player corra.. Y O Este en modo "Rage"
                {
                    rage = true; // Rage igual a true
                    canSeePlayer = true; // Podemos ver al Player
                    if (canSeePlayer) // SI vemos al Player 
                    {                              
                            navMeshAgent.SetDestination(playerTransform.position); // Dame como nuevo destino del Agente la posicion del Player  
                                                                                
                        if (playerIsInAttackRange) // SI Player esta en el rango de ataque
                        {
                            ///El Codigo de ataque va Aqui
                            ///
                            /// 
                            /// 
                            ///
                            navMeshAgent.SetDestination(transform.position);
                        }
                    }
                }
                else
                {
                    canSeePlayer = false; // No podemos ver al Player
                    rage = false; // Ya no esta en modo ira
                }

            }
            else // SI no esta en nuestro angulo entonces no lo vemos
            {
                canSeePlayer = false; // No podemos ver al Player
                rage = false; // Ya no esta en modo ira
            }
        }else if(canSeePlayer)//SI nuestro Player paso por la deteccion.. pero despues se salio..
        {
            canSeePlayer = false; // Ya no podemos ver al Player
            rage = false;  // Ya no esta en modo ira
        }


        if(canSeePlayer == false) // SI no podemos ver a nuestro Player
        {
            if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance) //SI la distancia del Agente es menor a la distancia en el cual el Agente se tiene que frenar
            {
                currentWayPointIndex = (currentWayPointIndex + 1) % wayPoints.Length; //Nuestra actual posicion la pasamos a la siguiente posicion , el operador "%" sirve para que cuando llegemos al ultimo destino se repita el ciclo ejemplo: 01234..01234
                navMeshAgent.SetDestination(wayPoints[currentWayPointIndex].position);//Dale al agente su nuevo destino
            }
        }


    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player")) // Si colisiono con un gameObject con etiqueta "Player"
        {
            rage = true; // Ira es igual a true
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
