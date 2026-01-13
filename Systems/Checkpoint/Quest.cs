using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "new Condition", menuName = "Scriptable Objects/Condition", order = 1)]
public class Quest : ScriptableObject
{
    public Condition[] conditions;

    //These are temporary to say the least :P
    public enum Type
    {
        FiresLit,
        GatherTorches,
        TorchesLit,
        FindArtifact,
        VillagersSaved,
        
        /*
        AnimalsSaved,
        FireHeld,
        BoingiesBoofed,*/
        
    }
    public bool CanLight(CheckPoint q)
    {
        bool light = true;
        foreach (var c in conditions)
            if (!c.GetCondition(q)) light = false;

        return light;

    }

    public string QuestProgress(CheckPoint q)
    {
        string progress = "";
        foreach(var c in conditions)
        {
            progress += c.GetProgress(q) + "\n";
        }

        return progress;
    }

    [System.Serializable]
    public class Condition
    {
        public int amount;
        public Type type;

        public bool GetCondition(CheckPoint q)
        {
            switch (type)
            {
                default: return true;
                case Type.FiresLit:
                    return amount <= GetFiresLit(q);
                
                case Type.GatherTorches:
                    return amount <= GetGatheredTorches(q);
                case Type.TorchesLit:
                    return amount <= GetTorchesLit(q);
                case Type.FindArtifact:
                    return amount <= GetArtifacts(q);
                case Type.VillagersSaved:
                    return amount <= GetHumansSaved(q);
                    /*
                case Type.AnimalsSaved:
                    return false;

                case Type.FireHeld:
                    return false;

                case Type.BoingiesBoofed:
                    return true;
                    */
            }
        }

        public ConditionDisplay GetProgress(CheckPoint q)
        {
            ConditionDisplay result = new ConditionDisplay();
            
            string s = "";
            string c = GetCondition(q) ? "<color=green>" : "<color=red>";
            const string undoColor = "</color>";

            switch (type)
            {
                case Type.FiresLit:
                    result.image = Resources.Load<Sprite>("Icons/Campfire");
                    result.currentCount = GetFiresLit(q);
                    result.totalCount = amount;
                    break;
                // case Type.GatherTorches:
                //     s = $"{c}{GetGatheredTorches(q)} / {amount}";
                //     break;
                case Type.TorchesLit:
                    
                    result.image = Resources.Load<Sprite>("Icons/Torch");
                    result.currentCount = GetTorchesLit(q);
                    result.totalCount = amount;
                    break;
                case Type.FindArtifact:
                    
                    result.image = q.artifacts[0].artifactSprite;
                    result.currentCount = GetArtifacts(q);
                    result.totalCount = amount;
                    break;
                case Type.VillagersSaved:
                    result.image = Resources.Load<Sprite>("Icons/Villager");
                    result.currentCount = GetHumansSaved(q);
                    result.totalCount = amount;
                    break;
                    /*
                case Type.AnimalsSaved:
                    return false;

                case Type.FireHeld:
                    return false;

                case Type.BoingiesBoofed:
                    return true;
                    */
            }

            return result;
        }

        public int GetFiresLit(CheckPoint q)
        {
            return q.campfires.Where(t => t.isLit).Count();
        }
        
        public int GetGatheredTorches(CheckPoint q)
        {
            return q.torches.Where(t => t.isGathered).Count();
        }
        
        public int GetTorchesLit(CheckPoint q)
        {
            return q.torches.Where(t => t.isLit).Count();
        }
        
        public int GetArtifacts(CheckPoint q)
        {
            return q.artifacts.Where(t => t.PickedUp).Count();
        }

        public int GetHumansSaved(CheckPoint q)
        {
            return q.restSpace.HumansResting;
        }
    }
}

public class ConditionDisplay
{
    public Sprite image;
    public int currentCount;
    public int totalCount;
}