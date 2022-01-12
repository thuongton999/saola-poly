using UnityEngine;

public class LivingEntity : MonoBehaviour {

    public int colourMaterialIndex;
    public Species species;
    public Material material;

    public Coord coord;
    //
    [HideInInspector]
    public int mapIndex;
    [HideInInspector]
    public Coord mapCoord;

    protected bool dead;

    public virtual void Init (Coord coord) {
        this.coord = coord;
        transform.position = Environment.tileCentres[coord.x, coord.y];

        // Set material to the instance material
        var meshRenderer = transform.GetComponentInChildren<MeshRenderer> ();
        for (int i = 0; i < meshRenderer.sharedMaterials.Length; i++)
        {
            if (meshRenderer.sharedMaterials[i] != material) continue;
            material = meshRenderer.materials[i];
            break;
        }
    }

    protected virtual void Die (CauseOfDeath cause) {
        if (dead) return;
        dead = true;
        Environment.RegisterDeath (this, cause);
        Destroy (gameObject);
    }

    public void KilledBy (LivingEntity killer) {
        Die (CauseOfDeath.Eaten);
    }
}