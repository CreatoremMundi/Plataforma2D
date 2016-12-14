using UnityEngine;
using System.Collections;

public class EnergyController : MonoBehaviour {
    public float startingEnergy;
    public float maxEnergy;

    public float CurrentEnergy { get; private set; }

    void Start () {
        if (startingEnergy < 0)
        {
            CurrentEnergy = 0;
        }
        else if (startingEnergy > maxEnergy)
        {
            CurrentEnergy = maxEnergy;
        }else
        {
            CurrentEnergy = startingEnergy;
        }
	}

    public bool Consume(float energy)
    {
        if(CurrentEnergy >= energy)
        {
            CurrentEnergy -= energy;
            return true;
        }

        return false;
    }
}
