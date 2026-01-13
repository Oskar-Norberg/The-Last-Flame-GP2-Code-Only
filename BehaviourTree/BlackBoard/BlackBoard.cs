using System.Collections.Generic;

public class BlackBoard
{
    private Dictionary<string, object> data = new Dictionary<string, object>();

    /**
     * Slightly over-engineered.
     * But this way encapsulates the data, and makes it easy to track where it is being altered.
     */
    public bool TryGetData(string key, out object value)
    {
        return data.TryGetValue(key, out value);
    }

    public void SetData(string key, object value)
    {
        data[key] = value;
    }
}
