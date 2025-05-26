export function DiagramPrintTool() {
    return (
        <>
        <div id="header">
    <div style="float: left; ">
        <div id="btnScale-outer"><div><a href="javascript:" title="Click to reset scale.">Scale</a>: </div><div id="btnScale"><div>&nbsp;</div></div><div id="btnScaleRes"></div></div>
    </div>
    <div style="float: right; ">
        <input type="button" id="btnGetImage" value="Get Image" />
        <input type="button" id="btnPrint" value="Print" />
    </div>
</div>
<canvas id="cvs"></canvas></>
    );
}