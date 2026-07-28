// TesterLab — TestRun (Create / Edit)
// Reads window.TRUN_INIT = { executionType: "Feature", targetIds: "12" } set inline by each view.
(function () {
  const init = window.TRUN_INIT || { executionType: 'Feature', targetIds: '' };

  const execTypeButtons = document.querySelectorAll('.trun-exectype');
  const hiddenExecType = document.getElementById('executionType');
  const hiddenTargetIds = document.getElementById('targetIdsHidden');
  const featureSelect = document.getElementById('featureSelect');
  const testCaseSelect = document.getElementById('testCaseSelect');
  const testCheckboxes = document.querySelectorAll('.test-checkbox');
  const featureCheckboxes = document.querySelectorAll('.feature-checkbox');
  const selectedCountEl = document.getElementById('selectedCount');

  function showSection(type) {
    document.querySelectorAll('.trun-target').forEach((s) => s.classList.add('d-none'));
    const map = {
      Feature: 'featureTarget',
      TestCase: 'testCaseTarget',
      Multiple: 'multipleTarget',
      FullRegression: 'regressionTarget',
    };
    const el = document.getElementById(map[type]);
    if (el) el.classList.remove('d-none');
  }

  function setActiveButton(type) {
    execTypeButtons.forEach((b) => b.classList.toggle('is-active', b.dataset.value === type));
  }

  function updateTargetIds() {
    const type = hiddenExecType.value;
    let value = '';

    switch (type) {
      case 'Feature':
        value = featureSelect ? featureSelect.value : '';
        break;
      case 'TestCase':
        value = testCaseSelect ? testCaseSelect.value : '';
        break;
      case 'Multiple': {
        const checked = document.querySelectorAll('.test-checkbox:checked');
        value = Array.from(checked).map((cb) => cb.value).join(',');
        break;
      }
      case 'FullRegression':
        value = '*';
        break;
      default:
        break;
    }

    if (hiddenTargetIds) hiddenTargetIds.value = value;
  }

  function updateSelectedCount() {
    if (!selectedCountEl) return;
    const count = document.querySelectorAll('.test-checkbox:checked').length;
    selectedCountEl.textContent = `${count} test(s) sélectionné(s)`;
  }

  function selectExecType(type, { silent } = {}) {
    hiddenExecType.value = type;
    setActiveButton(type);
    showSection(type);
    if (!silent) updateTargetIds();
  }

  execTypeButtons.forEach((btn) => {
    btn.addEventListener('click', () => selectExecType(btn.dataset.value));
  });

  featureSelect?.addEventListener('change', updateTargetIds);
  testCaseSelect?.addEventListener('change', updateTargetIds);

  featureCheckboxes.forEach((fcb) => {
    fcb.addEventListener('change', function () {
      const featureId = this.dataset.feature;
      document.querySelectorAll(`.test-checkbox[data-feature="${featureId}"]`).forEach((tcb) => {
        tcb.checked = this.checked;
      });
      updateSelectedCount();
      updateTargetIds();
    });
  });

  testCheckboxes.forEach((tcb) => {
    tcb.addEventListener('change', function () {
      const featureId = this.dataset.feature;
      const featureTests = document.querySelectorAll(`.test-checkbox[data-feature="${featureId}"]`);
      const checkedTests = document.querySelectorAll(`.test-checkbox[data-feature="${featureId}"]:checked`);
      const featureCheckbox = document.getElementById(`feature_${featureId}`);
      if (featureCheckbox) {
        featureCheckbox.checked = featureTests.length === checkedTests.length;
        featureCheckbox.indeterminate = checkedTests.length > 0 && checkedTests.length < featureTests.length;
      }
      updateSelectedCount();
      updateTargetIds();
    });
  });

  document.getElementById('selectAllTests')?.addEventListener('click', () => {
    testCheckboxes.forEach((cb) => { cb.checked = true; });
    featureCheckboxes.forEach((cb) => { cb.checked = true; cb.indeterminate = false; });
    updateSelectedCount();
    updateTargetIds();
  });

  document.getElementById('deselectAllTests')?.addEventListener('click', () => {
    testCheckboxes.forEach((cb) => { cb.checked = false; });
    featureCheckboxes.forEach((cb) => { cb.checked = false; cb.indeterminate = false; });
    updateSelectedCount();
    updateTargetIds();
  });

  // ---- Initial state ----
  document.addEventListener('DOMContentLoaded', () => {
    selectExecType(init.executionType || 'Feature', { silent: true });

    if (init.executionType === 'Feature' && featureSelect && init.targetIds) {
      featureSelect.value = init.targetIds;
    }
    if (init.executionType === 'TestCase' && testCaseSelect && init.targetIds) {
      testCaseSelect.value = init.targetIds;
    }
    if (init.executionType === 'Multiple' && init.targetIds) {
      const ids = init.targetIds.split(',').map((s) => s.trim()).filter(Boolean);
      ids.forEach((id) => {
        const cb = document.getElementById(`test_${id}`);
        if (cb) cb.checked = true;
      });
      featureCheckboxes.forEach((fcb) => {
        const featureId = fcb.dataset.feature;
        const featureTests = document.querySelectorAll(`.test-checkbox[data-feature="${featureId}"]`);
        const checkedTests = document.querySelectorAll(`.test-checkbox[data-feature="${featureId}"]:checked`);
        fcb.checked = featureTests.length > 0 && featureTests.length === checkedTests.length;
        fcb.indeterminate = checkedTests.length > 0 && checkedTests.length < featureTests.length;
      });
      updateSelectedCount();
    }
    if (hiddenTargetIds && init.executionType !== 'Multiple') {
      hiddenTargetIds.value = init.targetIds || (init.executionType === 'FullRegression' ? '*' : '');
    }
    updateTargetIds();
  });
})();
