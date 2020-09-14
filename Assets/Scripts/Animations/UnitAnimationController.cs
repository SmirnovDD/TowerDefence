using UnityEngine;
public class UnitAnimationController : MonoBehaviour
{
    public Animator Anim;

    private const string LowerBodyClipName = "LowerBody";
    private const string UpperBodyClipName = "UpperBody";
    private const string FullBodyClipName = "FullBody";

    private static readonly int _startAnimLowerBodyTrigger = Animator.StringToHash("StartAnimLowerBody");
    private static readonly int _stopAnimLowerBodyTrigger = Animator.StringToHash("StopAnimLowerBody");
    
    private static readonly int _startAnimUpperBodyTrigger = Animator.StringToHash("StartAnimUpperBody");
    private static readonly int _stopAnimUpperBodyTrigger = Animator.StringToHash("StopAnimUpperBody");

    private static readonly int _startAnimFullBodyTrigger = Animator.StringToHash("StartAnimFullBody");
    private static readonly int _stopAnimFullBodyTrigger = Animator.StringToHash("StopAnimFullBody");

    private AnimationClip _lowerBodyPreviousClip;
    private AnimationClip _upperBodyPreviousClip;

    private float _lowerBodyAnimationTimer;
    private float _upperBodyAnimationTimer;
    private float _fullBodyAnimationTimer;

    private bool _stopLowerBodyAnimation;
    private bool _stopUpperBodyAnimation;
    private bool _stopFullBodyAnimation;
    
    public enum AnimationBodyPart
    {
        LowerBody,
        UpperBody,
        FullBody
    }
    private void Update()
    {
        if (GameTimeManager.Me.GameIsPaused)
            Anim.speed = 0;
        else
            Anim.speed = 1;
        
        UpdateLowerBodyAnimationTimer();

        UpdateUpperBodyAnimationTimer();

        UpdateFullBodyAnimationTimer();
    }

    private void UpdateLowerBodyAnimationTimer()
    {
        if (_lowerBodyAnimationTimer > 0 && _stopLowerBodyAnimation)
            _lowerBodyAnimationTimer -= GameTimeManager.Me.DeltaTime;
        else if (_lowerBodyAnimationTimer <= 0 && _stopLowerBodyAnimation)
            StopAnimationClip(AnimationBodyPart.LowerBody);
    }

    private void UpdateUpperBodyAnimationTimer()
    {
        if (_upperBodyAnimationTimer > 0 && _stopUpperBodyAnimation)
            _upperBodyAnimationTimer -= GameTimeManager.Me.DeltaTime;
        else if (_upperBodyAnimationTimer <= 0 && _stopUpperBodyAnimation)
            StopAnimationClip(AnimationBodyPart.UpperBody);
    }

    private void UpdateFullBodyAnimationTimer()
    {
        if (_fullBodyAnimationTimer > 0 && _stopFullBodyAnimation)
            _fullBodyAnimationTimer -= GameTimeManager.Me.DeltaTime;
        else if (_fullBodyAnimationTimer <= 0 && _stopFullBodyAnimation)
            StopAnimationClip(AnimationBodyPart.FullBody);
    }
    
    public void ChangeAnimationClip(AnimationClip clip, AnimationBodyPart bodyPart)
    {
        RuntimeAnimatorController myController = Anim.runtimeAnimatorController;

        AnimatorOverrideController myOverrideController = myController as AnimatorOverrideController;
        if(myOverrideController != null)
            myController = myOverrideController.runtimeAnimatorController;

        AnimatorOverrideController animatorOverride = new AnimatorOverrideController
        {
            runtimeAnimatorController = myController
        };
        
        var clipOverrides = new AnimationClipOverrides(animatorOverride.overridesCount);

        UpdateAnimationClipsOnOtherLayers(bodyPart, animatorOverride, clip);
        
        animatorOverride.ApplyOverrides(clipOverrides);
        Anim.runtimeAnimatorController = animatorOverride;

        SetTriggerToStartAnimation(bodyPart);
        
        if (clip.isLooping == false)
            StartTimerForNonLoopedAnimationClip(bodyPart, clip);
        else
            StopTimerForLoopedAnimationClip(bodyPart);
    }

    private void UpdateAnimationClipsOnOtherLayers(AnimationBodyPart bodyPart, AnimatorOverrideController animatorOverride, AnimationClip clip)
    {
        if (bodyPart == AnimationBodyPart.LowerBody)
        {
            UpdateLowerBodyAnimationClip(animatorOverride, clip);
        }
        else if (bodyPart == AnimationBodyPart.UpperBody)
        {
            UpdateUpperBodyAnimationClip(animatorOverride, clip);
        }
        else if (bodyPart == AnimationBodyPart.FullBody)
        {
            UpdateFullBodyAnimationClip(animatorOverride, clip);
        }
    }

    private void UpdateLowerBodyAnimationClip(AnimatorOverrideController animatorOverride, AnimationClip clip)
    {
        animatorOverride[LowerBodyClipName] = clip;
        animatorOverride[UpperBodyClipName] = _upperBodyPreviousClip;
        StopAnimationClip(AnimationBodyPart.FullBody);
        _lowerBodyPreviousClip = clip;
    }
    
    private void UpdateUpperBodyAnimationClip(AnimatorOverrideController animatorOverride, AnimationClip clip)
    {
        animatorOverride[UpperBodyClipName] = clip;
        animatorOverride[LowerBodyClipName] = _lowerBodyPreviousClip;
        StopAnimationClip(AnimationBodyPart.FullBody);
        _upperBodyPreviousClip = clip;
    }

    private void UpdateFullBodyAnimationClip(AnimatorOverrideController animatorOverride, AnimationClip clip)
    {
        animatorOverride[FullBodyClipName] = clip;
        animatorOverride[LowerBodyClipName] = _lowerBodyPreviousClip;
        animatorOverride[UpperBodyClipName] = _upperBodyPreviousClip;
    }

    private void SetTriggerToStartAnimation(AnimationBodyPart bodyPart)
    {
        if (bodyPart == AnimationBodyPart.LowerBody)
        {
            Anim.ResetTrigger(_stopAnimLowerBodyTrigger);
            Anim.SetTrigger(_startAnimLowerBodyTrigger);
        }
        if (bodyPart == AnimationBodyPart.UpperBody)
        {
            Anim.ResetTrigger(_stopAnimUpperBodyTrigger);
            Anim.SetTrigger(_startAnimUpperBodyTrigger);
        }
        if (bodyPart == AnimationBodyPart.FullBody)
        {
            Anim.ResetTrigger(_stopAnimFullBodyTrigger);
            Anim.SetTrigger(_startAnimFullBodyTrigger);
        }
    }

    private void StartTimerForNonLoopedAnimationClip(AnimationBodyPart bodyPart, AnimationClip clip)
    {
        if (bodyPart == AnimationBodyPart.LowerBody)
        {
            _lowerBodyAnimationTimer = clip.length;
            _stopLowerBodyAnimation = true;
        }

        if (bodyPart == AnimationBodyPart.UpperBody)
        {
            _upperBodyAnimationTimer = clip.length;
            _stopUpperBodyAnimation = true;
        }
        
        if (bodyPart == AnimationBodyPart.FullBody)
        {
            _fullBodyAnimationTimer = clip.length;
            _stopFullBodyAnimation = true;
        }
    }

    private void StopTimerForLoopedAnimationClip(AnimationBodyPart bodyPart)
    {
        if (bodyPart == AnimationBodyPart.LowerBody)
            _stopLowerBodyAnimation = false;
        else if (bodyPart == AnimationBodyPart.UpperBody)
            _stopUpperBodyAnimation = false;
        else if (bodyPart == AnimationBodyPart.FullBody)
            _stopFullBodyAnimation = false;
    }
    public void StopAnimationClip(AnimationBodyPart bodyPart)
    {
        if (bodyPart == AnimationBodyPart.LowerBody)
        {
            Anim.ResetTrigger(_startAnimLowerBodyTrigger);
            Anim.SetTrigger(_stopAnimLowerBodyTrigger);
        }

        if (bodyPart == AnimationBodyPart.UpperBody)
        {
            Anim.ResetTrigger(_startAnimUpperBodyTrigger);
            Anim.SetTrigger(_stopAnimUpperBodyTrigger);
        }

        if (bodyPart == AnimationBodyPart.FullBody)
        {
            Anim.ResetTrigger(_startAnimFullBodyTrigger);
            Anim.SetTrigger(_stopAnimFullBodyTrigger);
        }

        StopTimerForLoopedAnimationClip(bodyPart);
    }
}
