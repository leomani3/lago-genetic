using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indi : MonoBehaviour
{
    public float speed;
    public float senseRadius;

    private GameObject currentTarget = null;
    private Material mat;
    private int eaten = 0;
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
        mat.color = new Color32((byte)Mathf.Clamp(speed * 5, 0, 255), 0, 0, 1);
    }

    // Update is called once per frame
    void Update()
    {
        FindClosestRessource();

        if (currentTarget != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, currentTarget.transform.position, speed * Time.deltaTime); ;
        }
    }

    /// <summary>
    /// Capte toutes les ressources autours et renvoie la plus proche dans currentTarget. S'il n'y a aucune ressource currentTarget sera null
    /// </summary>
    private void FindClosestRessource()
    {
        currentTarget = null;
        Debug.Log(currentTarget);
        Collider[] colliders = Physics.OverlapSphere(transform.position, senseRadius);
        //Debug.Log(colliders.Length);
        float min = 99999;
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.tag == "ressource")
            {
                if (Vector3.Distance(transform.position, colliders[i].gameObject.transform.position) < min)
                {
                    currentTarget = colliders[i].gameObject;
                    min = Vector3.Distance(transform.position, currentTarget.transform.position);
                }
            }
        }
    }

    /// <summary>
    /// Renvoie le nombre de ressource que l'individu a mangé
    /// </summary>
    /// <returns></returns>
    public int GetEaten()
    {
        return eaten;
    }

    public void SetEaten(int nb)
    {
        eaten = nb;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "ressource")
        {
            Destroy(collision.gameObject);
            eaten++;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, senseRadius);
    }
}
