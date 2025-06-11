// language=sql
const SlowestRunningQueries = `
  SELECT query,
         calls                                   AS executions,
         cast(mean_exec_time as DECIMAL(18, 2))  AS avg_execution_ms,
         cast(total_exec_time as DECIMAL(18, 2)) AS total_execution_ms
  FROM public.pg_stat_statements
  WHERE queryid is not null
  ORDER BY mean_exec_time DESC LIMIT 20;
`;
// language=sql
const MostFrequentQueries = `
  SELECT query,
         calls                                   AS executions,
         cast(mean_exec_time as DECIMAL(18, 2))  AS avg_execution_ms,
         cast(total_exec_time as DECIMAL(18, 2)) AS total_execution_ms
  FROM public.pg_stat_statements
  WHERE queryid is not null
  ORDER BY calls DESC LIMIT 20;
`;

// language=sql
const LongestTotalRunningQueries = `
  SELECT query,
         calls                                   AS executions,
         cast(mean_exec_time as DECIMAL(18, 2))  AS avg_execution_ms,
         cast(total_exec_time as DECIMAL(18, 2)) AS total_execution_ms
  FROM public.pg_stat_statements
  WHERE queryid is not null
  ORDER BY total_exec_time DESC LIMIT 20;
`;

// language=sql
const HighestDiskReadQueries = `
  SELECT query,
         calls                                   AS executions,
         shared_blks_read                        AS disk_reads,
         shared_blks_hit                         AS cache_hits,
         cast(mean_exec_time as DECIMAL(18, 2))  AS avg_execution_ms,
         cast(total_exec_time as DECIMAL(18, 2)) AS total_execution_ms
  FROM public.pg_stat_statements
  WHERE queryid is not null
  ORDER BY shared_blks_read DESC LIMIT 20;
`;
export type QueryMetricOption = { label: string; description: string; value: string };
export const QueryMetricOptions: QueryMetricOption[] = [
  {
    label: 'Slowest running queries',
    description: 'These queries have the highest average execution time per call. Consider investigating their execution plans using EXPLAIN (ANALYZE, BUFFERS) to identify slow joins, missing indexes, or inefficient scans. Optimizing these queries can significantly improve performance.',
    value: SlowestRunningQueries,
  },
  {
    label: 'Most frequent queries',
    description: 'These queries are executed most frequently, so even small performance gains can have a big impact. Review them for potential caching opportunities, batching, or rewriting to reduce the number of calls.',
    value: MostFrequentQueries,
  },
  {
    label: 'Longest total running queries',
    description: 'These queries consume the most total execution time in the database, contributing significantly to overall load. Focus on optimizing them first for maximum impact.',
    value: LongestTotalRunningQueries,
  },
  {
    label: 'Highest disk read queries',
    description: 'These queries cause the highest number of physical disk reads. High disk reads often indicate missing or inefficient indexes, or queries that scan large tables. Use EXPLAIN (ANALYZE, BUFFERS) to understand their plans and consider adding or optimizing indexes.',
    value: HighestDiskReadQueries,
  },
];
