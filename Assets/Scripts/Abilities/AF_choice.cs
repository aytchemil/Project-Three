using UnityEngine;

[System.Serializable]
public class AF_choice : AF
{
    public override string afname => "choice";
    public string choice;
    
    public AF_choice()
    {
        choice = "";
    }
    public AF_choice(string choice)
    {
        this.choice = choice;
    }
}
