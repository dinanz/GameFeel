using UnityEngine;

public class bgm : MonoBehaviour
{
    [SerializeField] AudioSource bgMusic;
    private bool togglePressed = false;
    public void musicTogglePress(){
        if(!togglePressed){
            bgMusic.Play();
            togglePressed = true;
        }else{
            bgMusic.Stop();
            togglePressed = false;
        }
    }
}
