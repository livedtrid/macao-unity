using UnityEngine;
using UnityEngine.SceneManagement;

namespace AR.DemoResources.Scripts
{
    public class Demo : MonoBehaviour
    {
        public void LoadScene(int index)
        {
            SceneManager.LoadScene(index);
        }

        private void Update()
        {
            if (Input.GetButtonDown("Cancel"))
            {
                SceneManager.LoadScene("Menu");
            }
        }
    }
}
