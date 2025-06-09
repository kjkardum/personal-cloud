import React, { useCallback, useMemo } from 'react';
import { Line } from 'react-chartjs-2';
import { PredefinedPrometheusQuery, usePostApiResourceBaseResourceByResourceIdPrometheusMutation } from '@/services/rtk/cloudyApi';


export const MetricChart = (props: {
  resourceId: string,
  query: PredefinedPrometheusQuery,
  range: {
    type?: 'relative' | 'absolute',
    start?: string,
    end?: string,
    step?: string,
  }
}) => {
  const [getMetrics] = usePostApiResourceBaseResourceByResourceIdPrometheusMutation();
  const customPrometheusReq = useCallback(
    async (start: string, end: string, step: any) => {
      const metrics = await getMetrics({
        resourceId: props.resourceId,
        queryPrometheusQuery: {
          query: props.query,
          start,
          end,
          step: String(step),
        },
      }).unwrap();
      return metrics.data;
    },
    [props.resourceId, props.query, getMetrics]
  );
  const memoedOptionsPrometheusChart = useMemo(()=>({
    plugins: {
      'datasource-prometheus': {
        query: customPrometheusReq,
        timeRange: {
          type: props.range?.type ?? 'relative',
          start: props.range?.start ?? -1 * 60 * 60 * 1000, // 1h ago
          end: props.range?.end ?? 0,   // now
          step: props.range?.step ?? '30s',
        },
      }
    },
    elements: {
      point: {
        radius: 1,
        hoverRadius: 5,
      }
    }}), [props.range.type, props.range.start, props.range.end, props.range.step, customPrometheusReq]);
  const memoedDummyDataPrometheusChart = useMemo(()=>({labels: [], datasets: []}), []);
  return (
    <>
      <Line data={memoedDummyDataPrometheusChart} options={memoedOptionsPrometheusChart as any} />
    </>
  )
}
