using UnityEngine;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{

	public VideoPlayer TitlePlayer, LoopPlayer;

	void Start()
	{
		TitlePlayer.loopPointReached += ChangeVideo;
		TitlePlayer.Play();
		LoopPlayer.Prepare();
	}

	public void ChangeVideo(VideoPlayer titlePlayer)
	{
		titlePlayer.gameObject.SetActive(false);
		LoopPlayer.Play();
	}
}
