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

    public void Add(float amount)
    {
        if (Mathf.Sign(amount) == -1)
            throw new System.Exception("O valor para ser adicionado à energia não pode ser negativo");

        float newEnergy = CurrentEnergy + amount;

        if (newEnergy > maxEnergy)
        {
            CurrentEnergy = maxEnergy;
        }
        else
        {
            CurrentEnergy = newEnergy;
        }
    }
}
