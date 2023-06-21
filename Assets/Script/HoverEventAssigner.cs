using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverEventAsigner : MonoBehaviour
{
    [SerializeField] private EventTrigger[] HoverButtons = default;
    // Start is called before the first frame update
    void Start()
    {
        // ホバーイベントを登録する
        foreach (var trigger in HoverButtons)
        {
            // ホバーイベントを定義
            var hoverEntry = new EventTrigger.Entry();
            hoverEntry.eventID = EventTriggerType.PointerEnter;
            var defaultScale = trigger.gameObject.transform.localScale;

            hoverEntry.callback.AddListener((data) => {

                trigger.gameObject.transform.localScale = defaultScale * 1.1f;
            });
            trigger.triggers.Add(hoverEntry);

            // ホバーアウトイベントを定義
            var hoverEntryOutentry = new EventTrigger.Entry();
            hoverEntryOutentry.eventID = EventTriggerType.PointerExit;

            hoverEntryOutentry.callback.AddListener((data) => {
                trigger.gameObject.transform.localScale = defaultScale;
            });
            trigger.triggers.Add(hoverEntryOutentry);
        }
    }
}