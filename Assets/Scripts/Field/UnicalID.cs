

public class UnicalID
{
    private static int Counter = 0;

    private int _id;
    
    private UnicalID() 
    {
        _id = Counter++;
    }

    public static UnicalID Create() 
    {
        return new UnicalID();
    }

    public bool Equals(UnicalID id) 
    {
        return _id == id._id;
    }
}
