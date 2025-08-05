{{- define "docker-homework-app.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" }}
{{- end }}

{{- define "docker-homework-app.fullname" -}}
{{- if .Values.fullnameOverride }}
{{- .Values.fullnameOverride | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- $name := default .Chart.Name .Values.nameOverride }}
{{- if contains $name .Release.Name }}
{{- .Release.Name | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- printf "%s-%s" .Release.Name $name | trunc 63 | trimSuffix "-" }}
{{- end }}
{{- end }}
{{- end }}

{{- define "docker-homework-app.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" }}
{{- end }}

{{- define "docker-homework-app.labels" -}}
helm.sh/chart: {{ include "docker-homework-app.chart" . }}
{{ include "docker-homework-app.selectorLabels" . }}
{{- if .Chart.AppVersion }}
app.kubernetes.io/version: {{ .Chart.AppVersion | quote }}
{{- end }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- end }}

{{- define "docker-homework-app.selectorLabels" -}}
app.kubernetes.io/name: {{ include "docker-homework-app.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end }}

{{- define "docker-homework-app.serviceName" -}}
{{ include "docker-homework-app.fullname" . }}-svc
{{- end }}

{{- define "docker-homework-app.containerPort" -}}
{{ .Values.containerPort | default 8000 }}
{{- end }}

{{- define "docker-homework-app.healthPath" -}}
{{ .Values.probes.path | default "/health" }}
{{- end }}