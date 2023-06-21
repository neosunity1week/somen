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
        // �z�o�[�C�x���g��o�^����
        foreach (var trigger in HoverButtons)
        {
            // �z�o�[�C�x���g���`
            var hoverEntry = new EventTrigger.Entry();
            hoverEntry.eventID = EventTriggerType.PointerEnter;
            var defaultScale = trigger.gameObject.transform.localScale;

            hoverEntry.callback.AddListener((data) => {

                trigger.gameObject.transform.localScale = defaultScale * 1.1f;
            });
            trigger.triggers.Add(hoverEntry);

            // �z�o�[�A�E�g�C�x���g���`
            var hoverEntryOutentry = new EventTrigger.Entry();
            hoverEntryOutentry.eventID = EventTriggerType.PointerExit;

            hoverEntryOutentry.callback.AddListener((data) => {
                trigger.gameObject.transform.localScale = defaultScale;
            });
            trigger.triggers.Add(hoverEntryOutentry);
        }
    }
}