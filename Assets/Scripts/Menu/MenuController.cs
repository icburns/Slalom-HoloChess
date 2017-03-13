using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Menu
{
    public class MenuController : MonoBehaviour {

        // Use this for initialization
        void Start ()
        {
            if (SceneManager.GetSceneByName("HoloChess").isLoaded)
            {
                SceneManager.UnloadSceneAsync("HoloChess");
            }
        }
	
        // Update is called once per frame
        void Update () {
	
        }

        void OnSelected(GameObject gameObject)
        {
            SceneManager.LoadScene("lobby");
        }
    }
}
