export function init(dotnetRef) {
    window.UnloadEventDotnetReference = dotnetRef;
    window.addEventListener("beforeunload", function (event) {
        if (window.UnloadEventDotnetReference != null) {
            window.UnloadEventDotnetReference.invokeMethodAsync('OnBeforePageUnload');
        }
    });
}