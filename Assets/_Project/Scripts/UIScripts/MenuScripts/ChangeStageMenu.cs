using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChangeStageMenu : MonoBehaviour
{
	[SerializeField] private Selectable _initialSelectable = default;
	[SerializeField] private GameObject _selectorTextPrefab = default;
	[SerializeField] private Transform _stageSelectorValues = default;
	[SerializeField] private Transform _musicSelectorValues = default;
	[SerializeField] private TextMeshProUGUI _musicText = default;
	[SerializeField] private TextMeshProUGUI _styleText = default;
	[SerializeField] private Image _stageImage = default;
	[SerializeField] private MusicSO _musicSO = default;
	[SerializeField] private StageSO[] _stagesSO = default;
	private StageSO _currentStage;
	private Selectable _previousSelectable;
	private Animator _changeStageAnimator;
	private EventSystem _currentEventSystem;
	private bool _isOpen;


	void Awake()
	{
		_changeStageAnimator = GetComponent<Animator>();
		_currentEventSystem = EventSystem.current;
		SetStageSelectorValues();
		SetMusicSelectorValues();
	}

	private void SetStageSelectorValues()
	{
		for (int i = 0; i < _stagesSO.Length; i++)
		{
			GameObject selector = Instantiate(_selectorTextPrefab, _stageSelectorValues);
			TextMeshProUGUI selectorText = selector.GetComponent<TextMeshProUGUI>();
			selectorText.text = _stagesSO[i].stageName;
			if (i == 0)
			{
				selector.SetActive(true);
			}
		}
		_currentStage = _stagesSO[0];
	}
	private void SetMusicSelectorValues()
	{
		for (int i = 0; i < _musicSO.songs.Length; i++)
		{
			GameObject selector = Instantiate(_selectorTextPrefab, _musicSelectorValues);
			TextMeshProUGUI selectorText = selector.GetComponent<TextMeshProUGUI>();
			selectorText.text = _musicSO.songs[i].ToString();
			if (i == 0)
			{
				selector.SetActive(true);
			}
		}
	}

	public void ChangeStageOpen()
	{
		if (!_isOpen)
		{
			_previousSelectable = _currentEventSystem.currentSelectedGameObject.GetComponent<Selectable>();
			_changeStageAnimator.Play("ChangeStageOpen");
			_initialSelectable.Select();
			_isOpen = true;
		}
	}

	public void ChangeStageClose()
	{
		if (_isOpen)
		{
			_changeStageAnimator.Play("ChangeStageClose");
			_previousSelectable.Select();
			_isOpen = false;
			_musicText.text = SceneSettings.MusicName;
			if (SceneSettings.Bit1)
			{
				_styleText.text = "1 Bit";
			}
			else 
			{
				_styleText.text = "Normal";
			}
		}
	}

	public void SetStage(int index)
	{
		_currentStage = _stagesSO[index];
		_stageImage.sprite = _stagesSO[index].colorStage;
		if (index == 0)
		{
			SceneSettings.RandomStage = true;
		}
		else
		{
			SceneSettings.StageIndex = index - 1;
		}
	}

	public void Set1Bit(int index)
	{
		if (index == 0)
		{
			_stageImage.sprite = _currentStage.colorStage;
			SceneSettings.Bit1 = false;
		}
		else if (index == 1)
		{
			_stageImage.sprite = _currentStage.bit1Stage;
			SceneSettings.Bit1 = true;
		}
	}

	public void SetMusic(int index)
	{
		SceneSettings.MusicName = _musicSO.songs[index].ToString();
	}

	public void SetTrainingMode(bool state)
	{
		SceneSettings.IsTrainingMode = state;
	}

	void OnDisable()
	{
		ChangeStageClose();
	}
}
