using Android.Content;
using Client.Services;
using System.Threading.Tasks;

public class AndroidSecureStorage : ISecureStorage
{
    private const string PreferenceName = "secure_prefs";
    private readonly ISharedPreferences _preferences;

    public AndroidSecureStorage(Context context)
    {
        _preferences = context.GetSharedPreferences(PreferenceName, FileCreationMode.Private);
    }

    public Task SaveAsync(string key, string value)
    {
        var editor = _preferences.Edit();
        editor.PutString(key, value);
        editor.Commit();
        return Task.CompletedTask;
    }

    public Task<string?> GetAsync(string key)
    {
        return Task.FromResult(_preferences.GetString(key, null));
    }
}
