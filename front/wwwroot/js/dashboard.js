// Graphique des tendances d'exécution
let executionTrendsChart = null;

function initExecutionTrendsChart(data) {
    const ctx = document.getElementById('executionTrendsChart');
    if (!ctx) return;

    const labels = data.map(d => {
        const date = new Date(d.date);
        return date.toLocaleDateString('fr-FR', { day: '2-digit', month: 'short' });
    });

    const passedData = data.map(d => d.passedRuns);
    const failedData = data.map(d => d.failedRuns);
    const totalData = data.map(d => d.totalRuns);

    if (executionTrendsChart) {
        executionTrendsChart.destroy();
    }

    executionTrendsChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [
                {
                    label: 'Tests réussis',
                    data: passedData,
                    backgroundColor: 'rgba(25, 135, 84, 0.85)',
                    borderColor: 'rgba(25, 135, 84, 1)',
                    borderWidth: 1,
                    borderRadius: 4,
                    stack: 'Stack 0'
                },
                {
                    label: 'Tests échoués',
                    data: failedData,
                    backgroundColor: 'rgba(220, 53, 69, 0.85)',
                    borderColor: 'rgba(220, 53, 69, 1)',
                    borderWidth: 1,
                    borderRadius: 4,
                    stack: 'Stack 0'
                },
                {
                    label: 'Volume total',
                    data: totalData,
                    type: 'line',
                    borderColor: 'rgba(13, 110, 253, 1)',
                    backgroundColor: 'rgba(13, 110, 253, 0.1)',
                    borderWidth: 3,
                    pointRadius: 5,
                    pointHoverRadius: 7,
                    pointBackgroundColor: 'rgba(13, 110, 253, 1)',
                    pointBorderColor: '#ffffff',
                    pointBorderWidth: 2,
                    tension: 0.3,
                    fill: false,
                    order: 0
                }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            interaction: {
                mode: 'index',
                intersect: false
            },
            plugins: {
                legend: {
                    display: true,
                    position: 'bottom',
                    labels: {
                        padding: 15,
                        usePointStyle: true,
                        font: {
                            size: 12,
                            weight: '500'
                        }
                    }
                },
                tooltip: {
                    backgroundColor: 'rgba(0, 0, 0, 0.85)',
                    padding: 12,
                    titleFont: {
                        size: 14,
                        weight: 'bold'
                    },
                    bodyFont: {
                        size: 13
                    },
                    bodySpacing: 6,
                    borderColor: 'rgba(255, 255, 255, 0.1)',
                    borderWidth: 1,
                    callbacks: {
                        title: function(context) {
                            return context[0].label;
                        },
                        label: function(context) {
                            let label = context.dataset.label || '';
                            if (label) {
                                label += ': ';
                            }
                            if (context.parsed.y !== null) {
                                label += context.parsed.y + ' test' + (context.parsed.y > 1 ? 's' : '');
                            }
                            return label;
                        },
                        footer: function(context) {
                            const index = context[0].dataIndex;
                            const total = totalData[index];
                            const passed = passedData[index];
                            if (total > 0) {
                                const rate = ((passed / total) * 100).toFixed(1);
                                return '\nTaux de réussite: ' + rate + '%';
                            }
                            return '';
                        }
                    }
                }
            },
            scales: {
                x: {
                    stacked: true,
                    grid: {
                        display: false
                    },
                    ticks: {
                        font: {
                            size: 11
                        }
                    }
                },
                y: {
                    stacked: true,
                    beginAtZero: true,
                    ticks: {
                        precision: 0,
                        font: {
                            size: 11
                        }
                    },
                    grid: {
                        color: 'rgba(0, 0, 0, 0.05)',
                        drawBorder: false
                    }
                }
            }
        }
    });
}

// Graphique du taux de réussite (Donut)
let successRateChart = null;

function initSuccessRateChart(data) {
    const ctx = document.getElementById('successRateChart');
    if (!ctx) return;

    if (successRateChart) {
        successRateChart.destroy();
    }

    successRateChart = new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: ['Réussis', 'Échoués', 'Autres'],
            datasets: [{
                data: [data.passed, data.failed, data.others],
                backgroundColor: [
                    'rgba(25, 135, 84, 0.85)',
                    'rgba(220, 53, 69, 0.85)',
                    'rgba(108, 117, 125, 0.85)'
                ],
                borderWidth: 2,
                borderColor: '#ffffff'
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            cutout: '70%',
            plugins: {
                legend: {
                    display: false
                },
                tooltip: {
                    backgroundColor: 'rgba(0, 0, 0, 0.8)',
                    padding: 12,
                    titleFont: {
                        size: 14,
                        weight: 'bold'
                    },
                    bodyFont: {
                        size: 13
                    },
                    callbacks: {
                        label: function(context) {
                            const label = context.label || '';
                            const value = context.parsed || 0;
                            const total = context.dataset.data.reduce((a, b) => a + b, 0);
                            const percentage = total > 0 ? ((value / total) * 100).toFixed(1) : 0;
                            return label + ': ' + value + ' (' + percentage + '%)';
                        }
                    }
                }
            }
        }
    });
}

// Fonction pour charger les données du graphique
async function loadChartData(period) {
    const chartLoading = document.getElementById('chartLoading');
    const chartCanvas = document.getElementById('executionTrendsChart');

    try {
        // Afficher le loader
        if (chartLoading) {
            chartLoading.style.display = 'block';
        }
        if (chartCanvas) {
            chartCanvas.style.display = 'none';
        }

        // Charger les nouvelles données
        const response = await fetch(`/Dashboard/GetChartData?type=execution-volume&days=${period}`);

        if (!response.ok) {
            throw new Error('Erreur lors du chargement des données');
        }

        const data = await response.json();

        // Cacher le loader
        if (chartLoading) {
            chartLoading.style.display = 'none';
        }
        if (chartCanvas) {
            chartCanvas.style.display = 'block';
        }

        // Mettre à jour le graphique
        initExecutionTrendsChart(data);

    } catch (error) {
        console.error('Erreur lors du chargement des données:', error);

        // Cacher le loader en cas d'erreur
        if (chartLoading) {
            chartLoading.style.display = 'none';
        }
        if (chartCanvas) {
            chartCanvas.style.display = 'block';
        }

        // Afficher un message d'erreur (optionnel)
        alert('Erreur lors du chargement des données du graphique');
    }
}

// Initialisation au chargement de la page
document.addEventListener('DOMContentLoaded', function() {
    // Initialiser le graphique des tendances avec les données initiales
    if (typeof initialTrendsData !== 'undefined' && initialTrendsData.length > 0) {
        const chartCanvas = document.getElementById('executionTrendsChart');
        if (chartCanvas) {
            chartCanvas.style.display = 'block';
            initExecutionTrendsChart(initialTrendsData);
        }
    }

    // Initialiser le graphique du taux de réussite
    if (typeof successRateData !== 'undefined') {
        initSuccessRateChart(successRateData);
    }

    // Gestion des boutons de période
    document.querySelectorAll('[data-period]').forEach(btn => {
        btn.addEventListener('click', async function() {
            const period = this.dataset.period;

            // Mettre à jour l'état actif
            document.querySelectorAll('[data-period]').forEach(b => b.classList.remove('active'));
            this.classList.add('active');

            // Charger les nouvelles données
            await loadChartData(period);
        });
    });

    // Auto-refresh toutes les 2 minutes (optionnel)
    // Décommentez si vous voulez un rafraîchissement automatique
    /*
    setInterval(async function() {
        const activePeriodBtn = document.querySelector('[data-period].active');
        if (activePeriodBtn) {
            const period = activePeriodBtn.dataset.period;
            await loadChartData(period);
        }
    }, 120000); // 2 minutes
    */
});
