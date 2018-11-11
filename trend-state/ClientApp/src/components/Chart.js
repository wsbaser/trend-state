import { format } from "d3-format";
import { timeFormat } from "d3-time-format";

import React from "react";
import PropTypes from "prop-types";

import { ChartCanvas, Chart } from "react-stockcharts";
import {
    BarSeries,
    AreaSeries,
    CandlestickSeries,
    LineSeries,
    MACDSeries,
} from "react-stockcharts/lib/series";
import { XAxis, YAxis } from "react-stockcharts/lib/axes";
import {
    CrossHairCursor,
    EdgeIndicator,
    CurrentCoordinate,
    MouseCoordinateX,
    MouseCoordinateY,
} from "react-stockcharts/lib/coordinates";

import { discontinuousTimeScaleProviderBuilder } from "react-stockcharts/lib/scale";
import { OHLCTooltip, MovingAverageTooltip, MACDTooltip } from "react-stockcharts/lib/tooltip";
import { ema, sma, macd } from "react-stockcharts/lib/indicator";
import { fitWidth } from "react-stockcharts/lib/helper";

function getMaxUndefined(calculators) {
    return calculators.map(each => each.undefinedLength()).reduce((a, b) => Math.max(a, b));
}
const LENGTH_TO_SHOW = 180;

class CandleStickChartPanToLoadMore extends React.Component {
    constructor(props) {
        super(props);
        const { data: inputData } = props;

        const ema26 = ema()
            .id(0)
            .options({ windowSize: 26 })
            .merge((d, c) => { d.ema26 = c; })
            .accessor(d => d.ema26);

        const ema12 = ema()
            .id(1)
            .options({ windowSize: 12 })
            .merge((d, c) => { d.ema12 = c; })
            .accessor(d => d.ema12);

        const maxWindowSize = getMaxUndefined([
            ema26,
            ema12
        ]);
        /* SERVER - START */
        const dataToCalculate = inputData.slice(-LENGTH_TO_SHOW - maxWindowSize);

        const calculatedData = ema26(ema12(dataToCalculate));
        const indexCalculator = discontinuousTimeScaleProviderBuilder().indexCalculator();

        // console.log(inputData.length, dataToCalculate.length, maxWindowSize)
        const { index } = indexCalculator(calculatedData);
        /* SERVER - END */

        const xScaleProvider = discontinuousTimeScaleProviderBuilder()
            .withIndex(index);
        const { data: linearData, xScale, xAccessor, displayXAccessor } = xScaleProvider(calculatedData.slice(-LENGTH_TO_SHOW));

        // console.log(head(linearData), last(linearData))
        // console.log(linearData.length)

        this.state = {
            ema26,
            ema12,
            linearData,
            data: linearData,
            xScale,
            xAccessor, displayXAccessor
        };
        this.handleDownloadMore = this.handleDownloadMore.bind(this);
    }
    handleDownloadMore(start, end) {
        if (Math.ceil(start) === end) return;
        // console.log("rows to download", rowsToDownload, start, end)
        const { data: prevData, ema26, ema12, macdCalculator, smaVolume50 } = this.state;
        const { data: inputData } = this.props;


        if (inputData.length === prevData.length) return;

        const rowsToDownload = end - Math.ceil(start);

        const maxWindowSize = getMaxUndefined([ema26,
            ema12
        ]);

        /* SERVER - START */
        const dataToCalculate = inputData
            .slice(-rowsToDownload - maxWindowSize - prevData.length, - prevData.length);

        const calculatedData = ema26(ema12(dataToCalculate));
        const indexCalculator = discontinuousTimeScaleProviderBuilder()
            .initialIndex(Math.ceil(start))
            .indexCalculator();
        const { index } = indexCalculator(
            calculatedData
                .slice(-rowsToDownload)
                .concat(prevData));
        /* SERVER - END */

        const xScaleProvider = discontinuousTimeScaleProviderBuilder()
            .initialIndex(Math.ceil(start))
            .withIndex(index);

        const { data: linearData, xScale, xAccessor, displayXAccessor } = xScaleProvider(calculatedData.slice(-rowsToDownload).concat(prevData));

        // console.log(linearData.length)
        setTimeout(() => {
            // simulate a lag for ajax
            this.setState({
                data: linearData,
                xScale,
                xAccessor,
                displayXAccessor,
            });
        }, 300);
    }
    render() {
        const { type, width, ratio } = this.props;
        const { data, ema26, ema12, macdCalculator, smaVolume50, xScale, xAccessor, displayXAccessor } = this.state;

        return (
            <ChartCanvas ratio={ratio} width={width} height={450}
                margin={{ left: 70, right: 70, top: 20, bottom: 30 }} type={type}
                seriesName="MSFT"
                data={data}
                xScale={xScale} xAccessor={xAccessor} displayXAccessor={displayXAccessor}
                onLoadMore={this.handleDownloadMore}>
                <Chart id={1} height={400}
                    yExtents={[d => [d.high, d.low], ema26.accessor(), ema12.accessor()]}
                    padding={{ top: 10, bottom: 20 }}>

                    <XAxis axisAt="bottom" orient="bottom" outerTickSize={0} />
                    <YAxis axisAt="right" orient="right" ticks={5} />

                    <MouseCoordinateY
                        at="right"
                        orient="right"
                        displayFormat={format(".2f")} />

                    <CandlestickSeries />
                    <LineSeries yAccessor={ema26.accessor()} stroke={ema26.stroke()} />
                    <LineSeries yAccessor={ema12.accessor()} stroke={ema12.stroke()} />

                    <CurrentCoordinate yAccessor={ema26.accessor()} fill={ema26.stroke()} />
                    <CurrentCoordinate yAccessor={ema12.accessor()} fill={ema12.stroke()} />

                </Chart>

                <CrossHairCursor />
            </ChartCanvas>
        );
    }
}

/*

*/

CandleStickChartPanToLoadMore.propTypes = {
    data: PropTypes.array.isRequired,
    width: PropTypes.number.isRequired,
    ratio: PropTypes.number.isRequired,
    type: PropTypes.oneOf(["svg", "hybrid"]).isRequired,
};

CandleStickChartPanToLoadMore.defaultProps = {
    type: "svg",
};

CandleStickChartPanToLoadMore = fitWidth(CandleStickChartPanToLoadMore);

export default CandleStickChartPanToLoadMore;