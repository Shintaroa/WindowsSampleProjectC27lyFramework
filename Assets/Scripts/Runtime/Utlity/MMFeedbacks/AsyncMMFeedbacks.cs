#if MOREMOUNTAINS_FEEDBACKS
using System.Collections;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using MoreMountains.Feedbacks;


    /// <summary>
    /// An async version of MMFeedbacks that provides async/await support for PlayFeedbacks and StopFeedbacks methods.
    /// This class extends MMFeedbacks and adds asynchronous versions of its core methods.
    /// </summary>
    [ExecuteAlways]
    [AddComponentMenu("More Mountains/Feedbacks/AsyncMMFeedbacks")]
    [DisallowMultipleComponent]
    public class AsyncMMFeedbacks : MMFeedbacks
    {
        private CancellationTokenSource _cancellationTokenSource = new ();

        public override void Initialization(GameObject owner)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            base.Initialization(owner);
        }

        /// <summary>
        /// Plays all feedbacks asynchronously using the MMFeedbacks' position as reference, and no attenuation.
        /// Waits for all feedbacks to complete before returning.
        /// </summary>
        /// <returns>A UniTask that completes when all feedbacks have finished playing</returns>
        public virtual async UniTask PlayFeedbacksAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await PlayFeedbacksAsync(this.transform.position, 1.0f,cancellationToken);
        }

        /// <summary>
        /// Plays all feedbacks asynchronously, specifying a position and attenuation.
        /// Waits for all feedbacks to complete before returning.
        /// </summary>
        /// <param name="position">Position for the feedbacks</param>
        /// <param name="attenuation">Attenuation factor for the feedbacks</param>
        /// <returns>A UniTask that completes when all feedbacks have finished playing</returns>
        public virtual async UniTask PlayFeedbacksAsync(Vector3 position, float attenuation = 1.0f,CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var linkedCts =
                   CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken))
            {
                // if this MMFeedbacks is disabled in any way, we stop and don't play
                if (!isActiveAndEnabled)
                {
                    return;
                }

                // if all MMFeedbacks are disabled globally, we stop and don't play
                if (!GlobalMMFeedbacksActive)
                {
                    return;
                }

                _startTime = Time.time;
                _holdingMax = 0f;
                _lastStartAt = _startTime;

                ResetFeedbacks();

                // Test if a pause or holding pause is found
                bool pauseFound = false;
                for (int i = 0; i < Feedbacks.Count; i++)
                {
                    if ((Feedbacks[i].Pause != null) && (Feedbacks[i].Active))
                    {
                        pauseFound = true;
                    }

                    if ((Feedbacks[i].HoldingPause == true) && (Feedbacks[i].Active))
                    {
                        pauseFound = true;
                    }
                }

                if (!pauseFound)
                {
                    // If no pause was found, we just play all feedbacks at once
                    IsPlaying = true;
                    for (int i = 0; i < Feedbacks.Count; i++)
                    {
                        Feedbacks[i].Play(position, attenuation);
                    }

                    await UniTask.WaitUntil(() =>
                    {
                        bool r = true;
                        foreach (var feedback in Feedbacks)
                        {
                            if (feedback)
                            {
                                if (feedback.FeedbackPlaying || feedback.FeedbackDelaying)
                                {
                                    r = false;
                                    break;
                                }

                            }
                        }

                        return r;
                    }, cancellationToken: linkedCts.Token);
                    IsPlaying = false;
                }
                else
                {
                    // If at least one pause was found
                    await PausedFeedbacksAsync(position, attenuation,linkedCts.Token);
                }
            }
        }

        /// <summary>
        /// An async version of PausedFeedbacksCo that handles feedbacks with pauses.
        /// </summary>
        /// <param name="position">Position for the feedbacks</param>
        /// <param name="attenuation">Attenuation factor for the feedbacks</param>
        /// <returns>A UniTask that completes when all paused feedbacks have finished playing</returns>
        protected virtual async UniTask PausedFeedbacksAsync(Vector3 position, float attenuation,CancellationToken cancellationToken = default(CancellationToken))
        {
            IsPlaying = true;
            for (int i = 0; i < Feedbacks.Count; i++)
            {
                // Handles holding pauses
                if ((Feedbacks[i].Active)
                    && ((Feedbacks[i].HoldingPause == true) || (Feedbacks[i].LooperPause == true)))
                {
                    // We stay here until all previous feedbacks have finished
                    await UniTask.WaitUntil(() => Time.time - _lastStartAt >= _holdingMax, cancellationToken: cancellationToken);

                    _holdingMax = 0f;
                    _lastStartAt = Time.time;
                }

                // Plays the feedback
                Feedbacks[i].Play(position, attenuation);

                // Handles pause
                if ((Feedbacks[i].Pause != null)
                    && (Feedbacks[i].Active))
                {
                    bool shouldPause = true;
                    if (Feedbacks[i].Chance < 100)
                    {
                        float random = Random.Range(0f, 100f);
                        if (random > Feedbacks[i].Chance)
                        {
                            shouldPause = false;
                        }
                    }

                    if (shouldPause)
                    {
                        await WaitShouldPause(Feedbacks[i].Pause).WithCancellation(cancellationToken);
                        _lastStartAt = Time.time;
                        _holdingMax = 0f;
                    }
                }

                // Updates holding max
                if (Feedbacks[i].Active)
                {
                    if (Feedbacks[i].Pause == null)
                    {
                        float feedbackDuration = Feedbacks[i].FeedbackDuration + Feedbacks[i].Timing.InitialDelay + Feedbacks[i].Timing.NumberOfRepeats * (Feedbacks[i].FeedbackDuration + Feedbacks[i].Timing.DelayBetweenRepeats);
                        _holdingMax = Mathf.Max(feedbackDuration, _holdingMax);
                    }
                }

                // Handles looper
                if ((Feedbacks[i].LooperPause == true)
                    && (Feedbacks[i].Active))
                {
                    MMFeedbackLooper looper = Feedbacks[i] as MMFeedbackLooper;
                    if (looper != null && ((looper.NumberOfLoopsLeft > 0) || looper.InInfiniteLoop))
                    {
                        // We determine the index we should start again at
                        bool loopAtLastPause = looper.LoopAtLastPause;
                        bool loopAtLastLoopStart = looper.LoopAtLastLoopStart;
                        int newi = 0;

                        for (int j = i - 1; j >= 0; j--)
                        {
                            // If we're at the start
                            if (j == 0)
                            {
                                newi = j - 1;
                                break;
                            }
                            // If we've found a pause
                            if ((Feedbacks[j].Pause != null)
                                && (Feedbacks[j].FeedbackDuration > 0f)
                                && loopAtLastPause && (Feedbacks[j].Active))
                            {
                                newi = j;
                                break;
                            }
                            // If we've found a looper start
                            if ((Feedbacks[j].LooperStart == true)
                                && loopAtLastLoopStart
                                && (Feedbacks[j].Active))
                            {
                                newi = j;
                                break;
                            }
                        }
                        i = newi;
                    }
                }
            }
            IsPlaying = false;
        }

        private IEnumerator WaitShouldPause(YieldInstruction yieldInstruction)
        {
            yield return yieldInstruction;
        }

        public override void StopFeedbacks(Vector3 position, float attenuation = 1.0f)
        {
            for (int i = 0; i < Feedbacks.Count; i++)
            {
                Feedbacks[i].Stop(this.transform.position, 1.0f);
            }
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            IsPlaying = false;
        }

        public  override void ResetFeedbacks()
        {
            for (int i = 0; i < Feedbacks.Count; i++)
            {
                Feedbacks[i].ResetFeedback();
            }
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            IsPlaying = false;
        }

        protected virtual bool AreAnyFeedbacksPlaying()
        {
            if (Feedbacks == null || Feedbacks.Count == 0)
            {
                return false;
            }

            foreach (var feedback in Feedbacks)
            {
                if (feedback != null && (feedback.FeedbackPlaying || feedback.FeedbackDelaying))
                {
                    return true;
                }
            }
            return false;
        }


    }
#endif