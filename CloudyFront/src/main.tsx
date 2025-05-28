import ReactDOM from 'react-dom/client';
import App from './App';

import { Chart as ChartJS, TimeSeriesScale, LinearScale, PointElement, LineElement, Tooltip } from "chart.js";
import 'chartjs-adapter-date-fns';
import ChartDatasourcePrometheusPlugin from 'chartjs-plugin-datasource-prometheus/dist/chartjs-plugin-datasource-prometheus.esm.js';
ChartJS.registry.plugins.register(ChartDatasourcePrometheusPlugin);
ChartJS.register(TimeSeriesScale, LinearScale, PointElement, LineElement, Tooltip);

ReactDOM.createRoot(document.getElementById('root')!).render(<App />);
