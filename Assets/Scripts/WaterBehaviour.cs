using UnityEngine;

public class WaterBehaviour : MonoBehaviour
{
    [Header("Behavior Settings")]
    public float PushStrength = 10;
    public float CohesiveStrength = 7;
    public float DiffusionPotentialPercentageLoss = 0.005f;
    public float EnergyLoss = 0.005f;

    public bool debugInfo = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        PushCohesionAdvectLevelDiffuse();
    }

    void PushCohesionAdvectLevelDiffuse()
    {
        Collider2D[] res = Physics2D.OverlapCircleAll(new Vector2(this.transform.position.x, this.transform.position.y), this.transform.localScale.x * 0.6f, LayerMask.GetMask("Liquid effect"));
        Push(res);
        Cohesion(res);
        Advect(res);
        Level(res);
        Diffusion(res);

        if (debugInfo)
        {
            Debug.DrawRay(this.transform.position, new Vector3(this.transform.localScale.x * 0.6f, 0), Color.white);
        }
    }

    void Push(Collider2D[] hits)
    {
        Vector2 dir = new Vector2(0, 0);

        int i = 0;
        foreach (var hitCollider in hits)
        {
            if (hitCollider != this.GetComponent<Collider2D>())
            {
                Vector2 pushA = this.transform.position - hitCollider.gameObject.GetComponent<Transform>().position;
                dir += pushA;
                i++;
            }
            else
                i++;
        }
        dir /= i;
        dir = dir.normalized * PushStrength;

        this.gameObject.GetComponent<Rigidbody2D>().AddForce(dir);
        if (debugInfo)
        {
            Debug.DrawRay(this.transform.position, new Vector3(dir.x, dir.y, 0), Color.red);
        }
    }

    void Cohesion(Collider2D[] hits)
    {
        Vector2 dir = new Vector2(0, 0);
        int i = 0;
        foreach (var hitCollider in hits)
        {
            Vector2 pushA = hitCollider.gameObject.GetComponent<Transform>().position;
            dir += pushA;
            i++;
        }
        dir /= i;
        dir = dir - new Vector2(this.transform.position.x, this.transform.position.y);
        dir = dir.normalized * CohesiveStrength;
        dir.x += Random.value * CohesiveStrength - (CohesiveStrength / 2);
        this.gameObject.GetComponent<Rigidbody2D>().AddForce(dir);
        if (debugInfo)
        {
            Debug.DrawRay(this.transform.position, new Vector3(dir.x, dir.y, 0), Color.blue);
        }
    }

    void Advect(Collider2D[] hits)
    {
        Vector2 dir = new Vector2(0, 0);
        int i = 0;
        foreach (var hitCollider in hits)
        {
            if (hitCollider != this.GetComponent<Collider2D>())
            {
                Vector2 pushA = hitCollider.gameObject.GetComponent<Rigidbody2D>().velocity;
                dir += pushA;
                i++;
            }
            else
                i++;
        }
        dir /= i;
        this.gameObject.GetComponent<Rigidbody2D>().AddForce(dir * (1 - EnergyLoss));
        if (debugInfo)
        {
            Debug.DrawRay(this.transform.position, new Vector3(dir.x, dir.y, 0), Color.green);
        }
    }

    void Level(Collider2D[] hits)
    {
        
        int i = hits.Length;
        int j = hits.Length;
        foreach (var hitCollider in hits)
        {
            if (hitCollider != this.GetComponent<Collider2D>())
            {
                if (hitCollider.gameObject.transform.position.y < this.transform.position.y)
                {
                    i -= 1;
                }
                if (hitCollider.gameObject.transform.position.x < this.transform.position.x)
                {
                    j -= 1;
                }
            }
        }
        if (i< hits.Length/2)
        {
            this.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(j - hits.Length, 0));
        }
    }

    void Diffusion(Collider2D[] hits)
    {
        Vector2 dir = new Vector2(0, 0);
        int i = 0;
        foreach (var hitCollider in hits)
        {
            if (hitCollider != this.GetComponent<Collider2D>())
            {
                if (this.GetComponent<Rigidbody2D>().velocity.magnitude > hitCollider.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude)
                {
                    hitCollider.gameObject.GetComponent<Rigidbody2D>().velocity += this.GetComponent<Rigidbody2D>().velocity * DiffusionPotentialPercentageLoss;
                    hitCollider.gameObject.GetComponent<Rigidbody2D>().velocity *= 1-EnergyLoss;
                    this.GetComponent<Rigidbody2D>().velocity *= 1-DiffusionPotentialPercentageLoss * 1 - EnergyLoss;
                }
                else
                {
                    this.GetComponent<Rigidbody2D>().velocity += hitCollider.gameObject.GetComponent<Rigidbody2D>().velocity * DiffusionPotentialPercentageLoss;
                    this.GetComponent<Rigidbody2D>().velocity *= 1-EnergyLoss;
                    hitCollider.gameObject.GetComponent<Rigidbody2D>().velocity *= 1-DiffusionPotentialPercentageLoss * 1 - EnergyLoss;
                }
            }
            else
                i++;
        }
    }
}
