{{- define "docker-homework-app.validate" -}}
  {{- if not .Values.jwt.issuer -}}
    {{- fail "values.jwt.issuer is required" -}}
  {{- end -}}
  {{- if not .Values.jwt.audience -}}
    {{- fail "values.jwt.audience is required" -}}
  {{- end -}}
  {{- if or (not .Values.jwt.accessTokenMinutes) (lt (int .Values.jwt.accessTokenMinutes) 1) -}}
    {{- fail "values.jwt.accessTokenMinutes must be >= 1" -}}
  {{- end -}}
  {{- if .Values.jwt.secret -}}
    {{- if lt (len .Values.jwt.secret) 32 -}}
      {{- fail "values.jwt.secret must be at least 32 characters for HS256" -}}
    {{- end -}}
  {{- end -}}
{{- end -}}