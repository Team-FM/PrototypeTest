using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class ClickEffect : MonoBehaviour
{
	public static ClickEffect instance
	{
		get
		{
			return Instantiate(Resources.Load<ClickEffect>("Prefab/ClickEffect"));
		}
	}

	#region Public Fields

	#endregion

	#region MonoBehaviour CallBacks

	private Material _material;
	private float _alpha;
	private float alpha
	{
		get => _alpha;
		set
		{
			_alpha = value;
			_material.SetFloat("_Alpha", _alpha);
		}
	}

	public CancellationTokenSource _source;
	public CancellationTokenSource _source2;
	public CancellationToken _token;
	public float _startAlpha;
	public float _stage1EndAlpha;
	public float _stage2EndAlpha;
	public float _startLerpTime;
	public float _endLerpTime;

	private void Awake()
	{
		_material = GetComponent<MeshRenderer>().material;
		_material = Instantiate(_material);
		GetComponent<MeshRenderer>().material = _material;
		_alpha = _material.GetFloat("_Alpha");

		_source = new CancellationTokenSource();
		_source2 = new CancellationTokenSource();

		_source2.Cancel();
	}

	private void OnEnable()
	{
		_token = _source.Token;
		alpha = _startAlpha;
		FadeInOut().Forget();
	}

	private void OnDisable()
	{
		_token = _source2.Token;
		alpha = _stage2EndAlpha;
	}

	#endregion

	#region Public Methods

	public async UniTaskVoid FadeInOut()
	{
		await AlphaChange(_startAlpha, _stage1EndAlpha, 1f / _startLerpTime);

		await AlphaChange(_stage1EndAlpha, _stage2EndAlpha, 1f / _endLerpTime);

		Destroy(gameObject);
	}

	public async UniTask AlphaChange(float startAlpha, float endAlpha, float lerpMul)
	{
		alpha = _startAlpha;
		float lerpT = 0f;

		while(lerpT < 1f)
		{
			lerpT += Time.deltaTime * lerpMul;

			alpha = Mathf.Lerp(startAlpha, endAlpha, lerpT);

			await UniTask.NextFrame(_token);
		}
	}

	#endregion
}